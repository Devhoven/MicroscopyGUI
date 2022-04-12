using System;
using peak.ipl;
using peak.core;
using peak.core.nodes;
using System.Drawing;
using System.Diagnostics;
using Image = peak.ipl.Image;
using Buffer = peak.core.Buffer;

namespace MicroscopeGUI
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
        NodeMap NodeMapRemoteDevice;

        bool Running = false;

        public void Start()
        {
            try
            {
                // Lock critical features to prevent them from changing during acquisition
                NodeMapRemoteDevice.FindNode<IntegerNode>("TLParamsLocked").SetValue(1);

                // Start acquisition
                DataStream.StartAcquisition();
                NodeMapRemoteDevice.FindNode<CommandNode>("AcquisitionStart").Execute();
                NodeMapRemoteDevice.FindNode<CommandNode>("AcquisitionStart").WaitUntilDone();
            }
            catch (Exception e)
            {
                UserInfo.SetErrorInfo("The camera failed to start");
                Debug.WriteLine("--- [AquisitionWorker] " + e.Message);
            }

            Loop();
        }

        void Loop()
        {
            int stride;

            uint errorCounter = 0;

            Bitmap image;
            Image iplImg;
            Buffer buffer;

            Running = true;
            while (Running)
            {
                try
                {
                    // Get buffer from device's datastream
                    buffer = DataStream.WaitForFinishedBuffer(1000);

                    // Create IDS peak IPL Image
                    iplImg = new Image((PixelFormatName)buffer.PixelFormat(), buffer.BasePtr(), buffer.Size(), buffer.Width(), buffer.Height());

                    // Debayering and converting IDS peak IPL Image to RGBa8 format
                    iplImg = iplImg.ConvertTo(PixelFormatName.BGRa8);

                    // Queue buffer so that it can be used again 
                    DataStream.QueueBuffer(buffer);

                    // Getting dimensions of the IDS peak IPL Image 
                    Width = (int)iplImg.Width();
                    Height = (int)iplImg.Height();
                    stride = (int)iplImg.PixelFormat().CalculateStorageSizeOfPixels(iplImg.Width());

                    // Fire the histogram updated event with the new data
                    HistogramUpdated(new Histogram(iplImg));

                    // Creating Bitmap from the IDS peak IPL Image
                    image = new Bitmap(Width, Height, stride, System.Drawing.Imaging.PixelFormat.Format32bppArgb, iplImg.Data());

                    if (ImageReceived != null)
                        ImageReceived(image);
                        
                    image.Dispose();
                    iplImg.Dispose();

                    // Resetting it to 0, since the current frame got sent successfully
                    errorCounter = 0;
                }
                catch (Exception e)
                {
                    errorCounter++;

                    Debug.WriteLine("--- [AquisitionWorker] " + e.Message);

                    // If more than 10 consecutive errors have been thrown a message is going to be shown to the user
                    if (errorCounter > 10)
                        UserInfo.SetErrorInfo("The camera failed to send more than 10 consecutive frames");
                }
            }
        }

        public void Stop() =>
            Running = false;

        public void SetDataStream(DataStream dataStream) =>
            DataStream = dataStream;

        public void SetNodemapRemoteDevice(NodeMap nodeMap) =>
            NodeMapRemoteDevice = nodeMap;
    }
}
