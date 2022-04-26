using std;
using System;
using peak.ipl;
using peak.core;
using System.Drawing;
using peak.core.nodes;
using System.Threading;
using System.Diagnostics;
using System.Windows.Threading;
using Image = peak.ipl.Image;
using Buffer = peak.core.Buffer;
using System.Drawing.Imaging;
using System.IO;

namespace MicroscopeGUI.IDSPeak
{
    public class AcquisitionWorker
    {
        // Event which is raised if a new image was received
        public delegate void ImageReceivedEventHandler(Bitmap image);
        public event ImageReceivedEventHandler ImageReceived;

        public delegate void HistogramUpdatedEventHandler(Histogram histogram);
        public event HistogramUpdatedEventHandler HistogramUpdated;

        // Contain the widht and height of the current image
        public int ImgWidth, ImgHeight;

        DataStream DataStream;
        NodeMap NodeMap;

        // Gets set to true if the user wants to save the current frame
        // Fires after the frame was saved by the acquisition thread
        public event Action SavedFrame;
        // Set by the SaveFrameTo method
        // Get's queried in the loop and if it's true, the current frame is saved
        bool SaveFrame = false;
        // Also set by the SaveFrameTo method
        // Specifies the path where the frame should be saved
        string SavePath;

        public bool UseColorCorrection = true;
        ColorCorrector ColorCorrector;
        HotpixelCorrection HotpixelCorrector;

        public bool Freeze = false;
        bool Running = false;

        Thread AcqThread;

        public void SetDataStream(DataStream dataStream)
        {
            DataStream = dataStream;
            NodeMap = DataStream.ParentDevice().RemoteDevice().NodeMaps()[0];

            ImgWidth = (int)NodeMap.FindNode<IntegerNode>("Width").Value();
            ImgHeight = (int)NodeMap.FindNode<IntegerNode>("Height").Value();
        }

        public void Start()
        {
            try
            {
                // Lock critical features to prevent them from changing during acquisition
                NodeMap.FindNode<IntegerNode>("TLParamsLocked").SetValue(1);

                // Start acquisition
                DataStream.StartAcquisition();
                NodeMap.FindNode<CommandNode>("AcquisitionStart").Execute();
                NodeMap.FindNode<CommandNode>("AcquisitionStart").WaitUntilDone();
            }
            catch (Exception e)
            {
                UserInfo.SetErrorInfo("The camera failed to start");
                Debug.WriteLine("--- [AquisitionWorker] " + e.Message);
            }

            InitColorCorrector();

            HotpixelCorrector = new HotpixelCorrection();

            Freeze = false;

            // Create acquisition worker thread that waits for new images from the camera
            AcqThread = new Thread(new ThreadStart(Loop));
            AcqThread.Start();
        }

        public void Stop()
        {
            Running = false;

            try
            {
                DataStream?.KillWait();
            }
            catch (Exception e)
            {
                Debug.WriteLine("--- [AquisitionWorker] " + e.Message);
            }

            AcqThread?.Join();
        }

        public void SaveFrameTo(string path)
        {
            SavePath = path;
            SaveFrame = true;
        }

        void Loop()
        {
            int stride;

            uint errorCounter = 0;

            Buffer buffer = null;
            Image iplImg = null;
            Bitmap currentFrameBitmap = null;
            Point2DCollection hotpixelVec = null;

            Running = true;
            while (Running)
            {
                try
                {
                    // Get buffer from device's datastream
                    buffer = DataStream.WaitForFinishedBuffer(1000);

                    ProcessIPLImage();

                    // Queue buffer so that it can be used again 
                    DataStream.QueueBuffer(buffer);

                    GetFrameBitmap();

                    CheckSaveFrame();

                    if (!Freeze)
                    {
                        // Disposing the images
                        currentFrameBitmap.Dispose();
                        iplImg.Dispose();
                    }

                    // Resetting it to 0, since the current frame got sent successfully
                    errorCounter = 0;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("--- [AquisitionWorker] " + e.Message);

                    ErrorHandling();
                }
            }

            void ProcessIPLImage()
            {
                if (Freeze)
                    return;

                // Create IDS peak IPL Image
                iplImg = new Image((PixelFormatName)buffer.PixelFormat(), buffer.BasePtr(), buffer.Size(), (uint)ImgWidth, (uint)ImgHeight);

                hotpixelVec = HotpixelCorrector.Detect(iplImg);
                iplImg = HotpixelCorrector.Correct(iplImg, hotpixelVec);

                // Debayering and converting IDS peak IPL Image to RGBa8 format
                iplImg = iplImg.ConvertTo(PixelFormatName.BGRa8, ConversionMode.Classic);

                // Applying the HQ matrix
                if (UseColorCorrection)
                    ColorCorrector.ProcessInPlace(iplImg);

                // Fire the histogram updated event with the new data
                HistogramUpdated(new Histogram(iplImg));
            }

            void GetFrameBitmap()
            {
                if (Freeze)
                    return;

                // Getting dimensions of the IDS peak IPL Image 
                stride = (int)iplImg.PixelFormat().CalculateStorageSizeOfPixels(iplImg.Width());

                // Creating Bitmap from the IDS peak IPL Image
                currentFrameBitmap = new Bitmap(ImgWidth, ImgHeight, stride, System.Drawing.Imaging.PixelFormat.Format32bppArgb, iplImg.Data());

                // Informing the UI 
                ImageReceived?.Invoke(currentFrameBitmap);
            }

            void CheckSaveFrame()
            {
                // If the user wants so save the current frame, this bool is set
                if (!SaveFrame)
                    return;

                if (currentFrameBitmap is null)
                {
                    UI.CurrentDispatcher.BeginInvoke(() => UserInfo.SetErrorInfo("You haven't captured a frame"), DispatcherPriority.Background);
                    return;
                }

                // Setting it to false, so only a single frame gets saved
                SaveFrame = false;

                // Saving the bitmap at the given path
                ApplyPostProcessing(currentFrameBitmap).Save(SavePath);

                // Informing the UI
                UI.CurrentDispatcher.BeginInvoke(() => SavedFrame.Invoke(), DispatcherPriority.Normal);
            }
        
            void ErrorHandling()
            {
                errorCounter++;

                // If more than 30 consecutive errors have been thrown the acqusition will be stopped
                if (errorCounter > 30)
                {
                    UI.CurrentDispatcher.BeginInvoke(() =>
                    {
                        UserInfo.SetErrorInfo("The camera failed to send more than 30 consecutive frames, stopping the acquisition");
                        Stop();
                    },
                        DispatcherPriority.Render);
                    errorCounter = 0;
                    Running = false;
                }
                // If more than 10 consecutive errors have been thrown a message is going to be shown to the user
                else if (errorCounter > 10)
                {
                    UI.CurrentDispatcher.BeginInvoke(()
                        => UserInfo.SetErrorInfo("The camera failed to send more than 10 consecutive frames"),
                        DispatcherPriority.Render);
                }
            }
        }

        void InitColorCorrector()
        {
            ColorCorrector = new ColorCorrector();

            try
            {
                EnumerationNode valueSelectorNode = NodeMap.FindNode<EnumerationNode>("ColorCorrectionMatrixValueSelector");
                FloatNode matrixValueNode = NodeMap.FindNode<FloatNode>("ColorCorrectionMatrixValue");

                var colorCorrectionFactors = new ColorCorrectionFactors(GetGain("Gain00"), GetGain("Gain01"), GetGain("Gain02"),
                                                                        GetGain("Gain10"), GetGain("Gain11"), GetGain("Gain12"),
                                                                        GetGain("Gain20"), GetGain("Gain21"), GetGain("Gain22"));

                float GetGain(string name)
                {
                    valueSelectorNode.SetCurrentEntry(name);
                    return (float)matrixValueNode.Value();
                }

                ColorCorrector.SetColorCorrectionFactors(colorCorrectionFactors);
            }
            catch (Exception e)
            {
                Debug.WriteLine("--- [AquisitionWorker] Couldn't set the color correction matrix \n" + e.Message);
            }
        }

        Bitmap ApplyPostProcessing(Bitmap frame)
        {
            // Extracting the brightness, contrast and RGB values (all values from 0 to 1) from the shader
            // Did this with a tuple, in order to r educe the Dispatcher calls
            (float b, float c, float red, float green, float blue)
                = UI.CurrentDispatcher.Invoke(() =>
                    (UI.FrameEffects.Brightness, UI.FrameEffects.Contrast, UI.FrameEffects.AmountR, UI.FrameEffects.AmountG, UI.FrameEffects.AmountB));

            float t = (1.0f - c) / 2.0f;

            // correcting brightness additively, contrast multiplicatively
            ColorMatrix cm = new ColorMatrix(new float[][]
            {
                new float[] {red * c, 0, 0, 0, 0},
                new float[] {0, green * c, 0, 0, 0},
                new float[] {0, 0, blue * c, 0, 0},
                new float[] {0, 0, 0, 1, 0},
                new float[] {b+t, b+t, b+t, 0, 0},
            });

            ImageAttributes attributes = new ImageAttributes();
            attributes.SetColorMatrix(cm);

            using (Graphics gr = Graphics.FromImage(frame))
            {
                gr.DrawImage(frame,
                           new Rectangle(0, 0, frame.Width, frame.Height),
                           0, 0,
                           frame.Width,
                           frame.Height,
                           GraphicsUnit.Pixel,
                           attributes);

                gr.Dispose();
            }
            attributes.Dispose();

            return frame;
        }
    }
}
