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

namespace MicroscopeGUI
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

        static AcquisitionWorker AcqWorker;

        static CamControl()
        {
            IsActive = true;

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

                // Initialize peak library
                Library.Initialize();
            }
            catch (Exception e)
            {
                UserInfo.SetErrorInfo("The camera failed to initialize");
                Debug.WriteLine("--- [BackEnd] Exception: " + e.Message);
            }
        }

        public static bool Start()
        {
            Debug.WriteLine("--- [BackEnd] Start");
            if (!OpenDevice())
                return false;

            AcqWorker.Start();

            // Retreiving the start and stop command nodes
            AcquisitionStartNode = NodeMap.FindNode<CommandNode>("AcquisitionStart");
            AcquisitionStopNode = NodeMap.FindNode<CommandNode>("AcquisitionStop");

            return true;
        }

        public static void Freeze()
            => AcqWorker.Freeze = true;

        public static void Unfreeze()
            => AcqWorker.Freeze = false;

        public static void Stop()
        {
            Debug.WriteLine("--- [BackEnd] Stop");
            IsActive = false;
            AcqWorker.Stop();

            CloseDevice();

            // Close peak library
            Library.Close();
        }

        public static bool OpenDevice()
        {
            Debug.WriteLine("--- [BackEnd] Open device");
            try
            {
                // Create instance of the device manager
                DeviceManager deviceManager = DeviceManager.Instance();

                // Update the device manager
                deviceManager.Update();

                // Return if no device was found
                if (!deviceManager.Devices().Any())
                {
                    Debug.WriteLine("--- [BackEnd] Error: No device found");
                    return false;
                }

                // Open the first openable device in the device manager's device list
                int deviceCount = deviceManager.Devices().Count();

                for (int i = 0; i < deviceCount; ++i)
                {
                    if (deviceManager.Devices()[i].IsOpenable())
                    {
                        Device = deviceManager.Devices()[i].OpenDevice(DeviceAccessType.Control);

                        // Stop after the first opened device
                        break;
                    }
                    else if (i == (deviceCount - 1))
                    {
                        Debug.WriteLine("--- [BackEnd] Error: Device could not be openend");
                        return false;
                    }
                }

                if (Device != null)
                {
                    // Check if any datastreams are available
                    var dataStreams = Device.DataStreams();

                    if (!dataStreams.Any())
                    {
                        Debug.WriteLine("--- [BackEnd] Error: Device has no DataStream");
                        return false;
                    }

                    // Open standard data stream
                    DataStream = dataStreams[0].OpenDataStream();

                    // Get nodemap of remote device for all accesses to the genicam nodemap tree
                    NodeMap = Device.RemoteDevice().NodeMaps()[0];

                    // To prepare for untriggered continuous image acquisition, load the default user set if available
                    // and wait until execution is finished
                    try
                    {
                        NodeMap.FindNode<EnumerationNode>("UserSetSelector").SetCurrentEntry("Default");
                        NodeMap.FindNode<CommandNode>("UserSetLoad").Execute();
                        NodeMap.FindNode<CommandNode>("UserSetLoad").WaitUntilDone();
                    }
                    catch
                    {
                        // UserSet is not available
                    }

                    // Trying to set the ColorCorrectionMatrix to "Hiqh Quality"
                    try
                    {
                        NodeMap.FindNode<EnumerationNode>("ColorCorrectionMatrix").SetCurrentEntry("HQ");
                    }
                    catch { }


                    // Get the payload size for correct buffer allocation
                    uint payloadSize = Convert.ToUInt32(NodeMap.FindNode<IntegerNode>("PayloadSize").Value());

                    // Get the minimum number of buffers that must be announced
                    uint bufferCountMax = DataStream.NumBuffersAnnouncedMinRequired();

                    // Allocate and announce image buffers and queue them
                    for (uint bufferCount = 0; bufferCount < bufferCountMax; ++bufferCount)
                    {
                        var buffer = DataStream.AllocAndAnnounceBuffer(payloadSize, IntPtr.Zero);
                        DataStream.QueueBuffer(buffer);
                    }

                    // Configure worker
                    AcqWorker.SetDataStream(DataStream);
                    AcqWorker.SetNodeMap(NodeMap);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("--- [BackEnd] Exception: " + e.Message);
                UserInfo.SetErrorInfo("The camera couldn't be opened");
                return false;
            }

            return true;
        }

        public static void CloseDevice()
        {
            // If device was opened, try to stop acquisition
            if (Device != null)
            {
                try
                {
                    NodeMap remoteNodeMap = Device.RemoteDevice().NodeMaps()[0];
                    remoteNodeMap.FindNode<CommandNode>("AcquisitionStop").Execute();
                    remoteNodeMap.FindNode<CommandNode>("AcquisitionStop").WaitUntilDone();
                }
                catch (Exception e)
                {
                    Debug.WriteLine("--- [BackEnd] Exception: " + e.Message);
                }
            }

            // If data stream was opened, try to stop it and revoke its image buffers
            if (DataStream != null)
            {
                try
                {
                    DataStream.KillWait();
                    DataStream.StopAcquisition(AcquisitionStopMode.Default);
                    DataStream.Flush(DataStreamFlushMode.DiscardAll);

                    foreach (var buffer in DataStream.AnnouncedBuffers())
                    {
                        DataStream.RevokeBuffer(buffer);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("--- [BackEnd] Exception: " + e.Message);
                }
            }

            try
            {
                // Unlock parameters after acquisition stop
                NodeMap.FindNode<IntegerNode>("TLParamsLocked").SetValue(0);
            }
            catch (Exception e)
            {
                Debug.WriteLine("--- [BackEnd] Exception: " + e.Message);
            }
        }

        public static void SetNodeValue(Action action)
        {
            AcquisitionStopNode.Execute();
            action();
            AcquisitionStartNode.Execute();
        }

        public static void LoadFromFile(string path)
        {
            // TODO
        }

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
