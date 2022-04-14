using System;
using peak.ipl;
using peak.core;
using peak.core.nodes;
using System.Drawing;
using System.Diagnostics;
using Image = peak.ipl.Image;
using Buffer = peak.core.Buffer;
using System.Threading;
using System.Windows.Threading;
using std;

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
        public int Width, Height;

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

        Bitmap CurrentFrameBitmap;

        public bool UseColorCorrection = true;
        ColorCorrector ColorCorrector;
        HotpixelCorrection HotpixelCorrector;

        public bool Freeze = false;
        bool Running = false;

        Thread AcqThread;

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

            // Create acquisition worker thread that waits for new images from the camera
            AcqThread = new Thread(new ThreadStart(Loop));
            AcqThread.Start();
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
       
        public void Stop()
        {
            Running = false;

            if (AcqThread.IsAlive)
                AcqThread.Join();
        }

        void Loop()
        {
            int stride;

            uint errorCounter = 0;

            Image iplImg = null;
            Buffer buffer = null;

            Running = true;
            while (Running)
            {
                try
                {
                    GetIPLImg(buffer);

                    // Fire the histogram updated event with the new data
                    if (!Freeze)
                        HistogramUpdated(new Histogram(iplImg));

                    GetFrameBitmap();
                    
                    CheckSaveFrame();

                    CurrentFrameBitmap.Dispose();
                    iplImg.Dispose();

                    // Resetting it to 0, since the current frame got sent successfully
                    errorCounter = 0;
                }
                catch (Exception e)
                {
                    errorCounter++;

                    Debug.WriteLine("--- [AquisitionWorker] " + e.Message);

                    // If more than 30 consecutive errors have been thrown the acqusition will be stopped
                    if (errorCounter > 30)
                    {
                        UI.CurrentDispatcher.BeginInvoke(() =>
                            {
                                UserInfo.SetErrorInfo("The camera failed to send more than 30 consecutive frames, stopping the acquisition");
                                Stop();
                            },
                            DispatcherPriority.Background);
                        errorCounter = 0;
                        break;
                    }
                    // If more than 10 consecutive errors have been thrown a message is going to be shown to the user
                    else if (errorCounter > 10)
                    {
                        UI.CurrentDispatcher.BeginInvoke(()
                            => UserInfo.SetErrorInfo("The camera failed to send more than 10 consecutive frames"),
                            DispatcherPriority.Background);
                    }
                }
            }

            void GetIPLImg(Buffer buffer)
            {
                // Get buffer from device's datastream
                buffer = DataStream.WaitForFinishedBuffer(1000);

                // Create IDS peak IPL Image
                iplImg = new Image((PixelFormatName)buffer.PixelFormat(), buffer.BasePtr(), buffer.Size(), buffer.Width(), buffer.Height());

                Point2DCollection hotpixelVec = HotpixelCorrector.Detect(iplImg);
                iplImg = HotpixelCorrector.Correct(iplImg, hotpixelVec);

                // Debayering and converting IDS peak IPL Image to RGBa8 format
                iplImg = iplImg.ConvertTo(PixelFormatName.BGRa8, ConversionMode.Classic);

                // Queue buffer so that it can be used again 
                DataStream.QueueBuffer(buffer);

                // Applying the HQ matrix
                if (UseColorCorrection)
                    ColorCorrector.ProcessInPlace(iplImg);
            }
            
            void GetFrameBitmap()
            {
                // Getting dimensions of the IDS peak IPL Image 
                Width = (int)iplImg.Width();
                Height = (int)iplImg.Height();
                stride = (int)iplImg.PixelFormat().CalculateStorageSizeOfPixels(iplImg.Width());

                // Creating Bitmap from the IDS peak IPL Image
                CurrentFrameBitmap = new Bitmap(Width, Height, stride, System.Drawing.Imaging.PixelFormat.Format32bppArgb, iplImg.Data());

                // Would be null if nothing subscribed to the event
                if (ImageReceived != null && !Freeze)
                    ImageReceived(CurrentFrameBitmap);
            }

            void CheckSaveFrame()
            {
                // If the user wants so save the current frame, this bool is set
                if (!SaveFrame)
                    return;

                // Setting it to false, so only a single frame gets saved
                SaveFrame = false;

                // Saving the bitmap at the given path
                CurrentFrameBitmap.Save(SavePath);

                // Informing the UI
                UI.CurrentDispatcher.BeginInvoke(() => SavedFrame.Invoke(), DispatcherPriority.Background);
            }
        }

        public void SetDataStream(DataStream dataStream) 
            => DataStream = dataStream;

        public void SetNodeMap(NodeMap nodeMap) 
            => NodeMap = nodeMap;

        public void SaveFrameTo(string path)
        {
            SavePath = path;
            SaveFrame = true;
        }
    }
}
