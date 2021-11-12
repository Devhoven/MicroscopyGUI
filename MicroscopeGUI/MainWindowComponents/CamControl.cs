/*!
 * \file    BackEnd.cs
 * \author  IDS Imaging Development Systems GmbH
 * \date    2020-02-01
 * \since   1.1.6
 *
 * \version 1.0.0
 *
 * Copyright (C) 2020 - 2021, IDS Imaging Development Systems GmbH.
 *
 * The information in this document is subject to change without notice
 * and should not be construed as a commitment by IDS Imaging Development Systems GmbH.
 * IDS Imaging Development Systems GmbH does not assume any responsibility for any errors
 * that may appear in this document.
 *
 * This document, or source code, is provided solely as an example of how to utilize
 * IDS Imaging Development Systems GmbH software libraries in a sample application.
 * IDS Imaging Development Systems GmbH does not assume any responsibility
 * for the use or reliability of any portion of this document.
 *
 * General permission to copy or modify is hereby granted.
 */

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

namespace MicroscopeGUI
{
    static class CamControl
    {
        // Event which is raised if a new image was received
        public delegate void ImageReceivedEventHandler(Bitmap image);
        public static event ImageReceivedEventHandler ImageReceived;

        public delegate void HistogramUpdatedEventHandler(Histogram histogram);
        public static event HistogramUpdatedEventHandler HistogramUpdated;

        public static bool IsActive;

        public static int Width { get => AcqWorker.Width; }
        public static int Height { get => AcqWorker.Height; }

        static AcquisitionWorker AcqWorker;
        static Thread AcqThread;

        static Device Device;
        static DataStream DataStream;
        static NodeMap NodeMapRemoteDevice;

        static CamControl()
        {
            IsActive = true;

            Initialize();
        }

        public static void Initialize()
        {
            try
            {
                // Create acquisition worker thread that waits for new images from the camera
                AcqWorker = new AcquisitionWorker();
                AcqThread = new Thread(new ThreadStart(AcqWorker.Start));

                AcqWorker.ImageReceived += AcquisitionWorker_ImageReceived;
                AcqWorker.HistogramUpdated += AcquisitionWorker_HistogramUpdated;

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

            // Start thread execution
            AcqThread.Start();

            return true;
        }

        public static void Freeze()
        {
            try
            {
                NodeMapRemoteDevice.FindNode<CommandNode>("AcquisitionStop").Execute();
            }
            catch (Exception e)
            {
                Debug.WriteLine("--- [BackEnd] The user tried to freeze a frozen cam: " + e.Message);
            }
        }

        public static void Unfreeze()
        {
            try
            {
                NodeMapRemoteDevice.FindNode<CommandNode>("AcquisitionStart").Execute();
            }
            catch (Exception e)
            {
                Debug.WriteLine("--- [BackEnd] The user tried to start a live cam: " + e.Message);
            }
        }

        public static void Stop()
        {
            Debug.WriteLine("--- [BackEnd] Stop");
            IsActive = false;
            AcqWorker.Stop();

            if (AcqThread.IsAlive)
                AcqThread.Join();

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
                    NodeMapRemoteDevice = Device.RemoteDevice().NodeMaps()[0];

                    // To prepare for untriggered continuous image acquisition, load the default user set if available
                    // and wait until execution is finished
                    try
                    {
                        NodeMapRemoteDevice.FindNode<EnumerationNode>("UserSetSelector").SetCurrentEntry("Default");
                        NodeMapRemoteDevice.FindNode<CommandNode>("UserSetLoad").Execute();
                        NodeMapRemoteDevice.FindNode<CommandNode>("UserSetLoad").WaitUntilDone();
                    }
                    catch
                    {
                        // UserSet is not available
                    }

                    // Get the payload size for correct buffer allocation
                    uint payloadSize = Convert.ToUInt32(NodeMapRemoteDevice.FindNode<IntegerNode>("PayloadSize").Value());

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
                    AcqWorker.SetNodemapRemoteDevice(NodeMapRemoteDevice);
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
                    var remoteNodeMap = Device.RemoteDevice().NodeMaps()[0];
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
                NodeMapRemoteDevice.FindNode<IntegerNode>("TLParamsLocked").SetValue(0);
            }
            catch (Exception e)
            {
                Debug.WriteLine("--- [BackEnd] Exception: " + e.Message);
            }
        }

        public static void SaveToFile(string path)
        {
            NodeMapRemoteDevice.StoreToFile(path);
        }

        public static ControlCon GetControlCon(StackPanel parent) 
            => new ControlCon(parent, NodeMapRemoteDevice);

        static void AcquisitionWorker_ImageReceived(Bitmap image)
            => ImageReceived(image);
    
        static void AcquisitionWorker_HistogramUpdated(Histogram histogram) 
            => HistogramUpdated(histogram);
    }
}
