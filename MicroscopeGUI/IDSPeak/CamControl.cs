using peak;
using System;
using peak.ipl;
using peak.core;
using System.Linq;
using System.Drawing;
using peak.core.nodes;
using System.Threading;
using System.Diagnostics;
using System.Windows.Controls;
using System.Text;
using System.Collections.Generic;
using MicroscopeGUI.MainWindowComponents.Controls;

namespace MicroscopeGUI.IDSPeak
{
    static class CamControl
    {
        // Event which is raised if a new image was received
        public delegate void ImageReceivedEventHandler(Bitmap image);
        public static event ImageReceivedEventHandler ImageReceived;

        public delegate void HistogramUpdatedEventHandler(Histogram histogram);
        public static event HistogramUpdatedEventHandler HistogramUpdated;

        public static event Action SavedFrame;

        public static bool IsActive;

        public static int Width => AcqWorker.Width;

        public static bool UseColorCorrection
        {
            get => AcqWorker.UseColorCorrection;
            set => AcqWorker.UseColorCorrection = value;
        }

        static Device Device;
        static DataStream DataStream;
        static NodeMap NodeMap;

        static CommandNode AcquisitionStartNode;
        static CommandNode AcquisitionStopNode;

        static readonly AcquisitionWorker AcqWorker;

        static CamControl()
        {
            AcqWorker = new AcquisitionWorker();

            Initialize();
        }

        public static void Initialize()
        {
            try
            {
                AcqWorker.ImageReceived += AcqWorker_ImageReceived;
                AcqWorker.HistogramUpdated += AcqWorker_HistogramUpdated;
                AcqWorker.SavedFrame += AcqWorker_SavedFrame;
            }
            catch (Exception e)
            {
                UserInfo.SetErrorInfo("The camera failed to initialize");
                Debug.WriteLine("--- [BackEnd] Exception: " + e.Message);
            }
        }

        public static bool Start()
        {
            IDSDeviceManager.DeviceInfo? result = IDSDeviceManager.OpenDevice();

            if (result is null 
                || result.Value.Device is null 
                || result.Value.NodeMap is null 
                || result.Value.DataStream is null)
                return false;

            Device = result.Value.Device;
            NodeMap = result.Value.NodeMap;
            DataStream = result.Value.DataStream;

            // Configure worker
            AcqWorker.SetDataStream(DataStream);
            AcqWorker.SetNodeMap(NodeMap);

            // Retreiving the start and stop command nodes
            AcquisitionStartNode = NodeMap.FindNode<CommandNode>("AcquisitionStart");
            AcquisitionStopNode = NodeMap.FindNode<CommandNode>("AcquisitionStop");

            // Starting the acquisition
            AcqWorker.Start();

            IsActive = true;
            return true;
        }

        public static void Freeze()
        {
            AcqWorker.Freeze = true;
            Stop();
        }

        public static void Unfreeze()
        {
            AcqWorker.Freeze = false;
            Start();
        }

        public static void Stop()
        {
            IsActive = false;
            AcqWorker.Stop();

            try
            {
                AcquisitionStopNode.Execute();
                AcquisitionStopNode.WaitUntilDone();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
            }

            try
            {
                IDSDeviceManager.CloseDevice(new IDSDeviceManager.DeviceInfo(Device, NodeMap, DataStream));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
            }
        }

        public static void SetNodeValue(Action action)
        {
            AcquisitionStopNode.Execute();
            action();
            AcquisitionStartNode.Execute();
        }

        public static void LoadFromFile(string path)
        { }

        public static void SaveToFile(string path)
            => NodeMap.StoreToFile(path);

        public static void SafeFrameTo(string path)
            => AcqWorker.SaveFrameTo(path);

        public static ControlCon GetControlCon() 
            => new ControlCon(NodeMap);

        static void AcqWorker_ImageReceived(Bitmap image)
            => ImageReceived(image);
    
        static void AcqWorker_HistogramUpdated(Histogram histogram) 
            => HistogramUpdated(histogram);

        static void AcqWorker_SavedFrame()
            => SavedFrame();
    }
}
