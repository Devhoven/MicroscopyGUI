﻿using System;
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
            int Stride;

            uint ErrorCounter = 0;

            Bitmap Image;
            Image IPLImg;
            Buffer Buffer;

            DateTime Before = DateTime.Now;
            DateTime After = Before;

            Running = true;
            while (Running)
            {
                try
                {
                    // Get buffer from device's datastream
                    Buffer = DataStream.WaitForFinishedBuffer(1000);

                    // Create IDS peak IPL Image
                    IPLImg = new Image((PixelFormatName)Buffer.PixelFormat(), Buffer.BasePtr(), Buffer.Size(), Buffer.Width(), Buffer.Height());

                    // Debayering and converting IDS peak IPL Image to RGBa8 format
                    IPLImg = IPLImg.ConvertTo(PixelFormatName.BGRa8);

                    // Queue buffer so that it can be used again 
                    DataStream.QueueBuffer(Buffer);

                    // Getting dimensions of the IDS peak IPL Image 
                    Width = (int)IPLImg.Width();
                    Height = (int)IPLImg.Height();
                    Stride = (int)IPLImg.PixelFormat().CalculateStorageSizeOfPixels(IPLImg.Width());

                    // Fire the histogram updated event with the new data
                    HistogramUpdated(new Histogram(IPLImg));

                    // Creating Bitmap from the IDS peak IPL Image
                    Image = new Bitmap(Width, Height, Stride, System.Drawing.Imaging.PixelFormat.Format32bppArgb, IPLImg.Data());

                    if (ImageReceived != null)
                        ImageReceived(Image);

                    Image.Dispose();
                    IPLImg.Dispose();

                    // Resetting it to 0, since the current frame got sent successfully
                    ErrorCounter = 0;
                }
                catch (Exception e)
                {
                    ErrorCounter++;

                    Debug.WriteLine("--- [AquisitionWorker] " + e.Message);

                    // If more than 10 consecutive errors have been thrown a message is going to be shown to the user
                    if (ErrorCounter > 10)
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
