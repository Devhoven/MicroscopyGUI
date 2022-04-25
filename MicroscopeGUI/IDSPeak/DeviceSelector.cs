using peak;
using peak.core;
using peak.core.nodes;
using std;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroscopeGUI.IDSPeak
{
    static class DeviceSelector
    {
        public struct DeviceInfo
        {
            public Device Device;
            public NodeMap NodeMap;
            public DataStream DataStream;

            public DeviceInfo(Device device, NodeMap nodeMap, DataStream dataStream)
            {
                Device = device;
                NodeMap = nodeMap;
                DataStream = dataStream;
            }
        }

        public static DeviceInfo? OpenDevice()
        {
            // Initialize peak library
            Library.Initialize();

            DeviceInfo result = new DeviceInfo();

            Debug.WriteLine("--- [BackEnd] Open device");
            try
            {
                result.Device = SelectFirstDevice();

                if (result.Device == null)
                {
                    Debug.WriteLine("--- [BackEnd] Error: Device could not be openend");
                    return null;
                }

                result.DataStream = OpenDataStream(result.Device);

                result.NodeMap = OpenNodeMap(result.Device);

                InitializeNodeMap(result.NodeMap);

                InitializeDataStream(result.DataStream, result.NodeMap);
            }
            catch (Exception e)
            {
                Debug.WriteLine("--- [BackEnd] Exception: " + e.Message);
                UserInfo.SetErrorInfo("The camera couldn't be opened");
                return null;
            }

            return result;
        }

        static Device SelectFirstDevice()
        {
            Device result = null;

            // Get the instance of the device manager
            DeviceManager deviceManager = DeviceManager.Instance();

            // Update the device manager
            deviceManager.Update();

            // Return if no device was found
            if (!deviceManager.Devices().Any())
            {
                Debug.WriteLine("--- [BackEnd] Error: No device found");
                return null;
            }

            // Open the first openable device in the device manager's device list
            int deviceCount = deviceManager.Devices().Count();

            for (int i = 0; i < deviceCount; i++)
            {
                if (deviceManager.Devices()[i].IsOpenable())
                {
                    result = deviceManager.Devices()[i].OpenDevice(DeviceAccessType.Control);

                    // Stop after the first opened device
                    break;
                }
            }
            
            return result;
        }

        static DataStream OpenDataStream(Device device)
        {
            // Check if any datastreams are available
            DataStreamDescriptorCollection dataStreams = device.DataStreams();

            if (!dataStreams.Any())
            {
                Debug.WriteLine("--- [BackEnd] Error: Device has no DataStream");
                return null;
            }

            // Open standard data stream
            return dataStreams[0].OpenDataStream();
        }

        // Get nodemap of remote device for all accesses to the genicam nodemap tree
        static NodeMap OpenNodeMap(Device device)
            => device.RemoteDevice().NodeMaps()[0];

        static void InitializeNodeMap(NodeMap nodeMap)
        {
            // To prepare for untriggered continuous image acquisition, load the default user set if available
            // and wait until execution is finished
            try
            {
                nodeMap.FindNode<EnumerationNode>("UserSetSelector").SetCurrentEntry("Default");
                nodeMap.FindNode<CommandNode>("UserSetLoad").Execute();
                nodeMap.FindNode<CommandNode>("UserSetLoad").WaitUntilDone();
            }
            catch
            {
                // UserSet is not available
            }
        }

        static void InitializeDataStream(DataStream dataStream, NodeMap nodeMap)
        {
            // Get the payload size for correct buffer allocation
            uint payloadSize = Convert.ToUInt32(nodeMap.FindNode<IntegerNode>("PayloadSize").Value());

            // Get the minimum number of buffers that must be announced
            uint bufferCountMax = dataStream.NumBuffersAnnouncedMinRequired();

            // Allocate and announce image buffers and queue them
            for (uint bufferCount = 0; bufferCount < bufferCountMax; ++bufferCount)
            {
                var buffer = dataStream.AllocAndAnnounceBuffer(payloadSize, IntPtr.Zero);
                dataStream.QueueBuffer(buffer);
            }
        }

        public static void CloseDevice(DeviceInfo deviceInfo)
        {
            // If data stream was opened, try to stop it and revoke its image buffers
            if (deviceInfo.DataStream != null)
            {
                try
                {
                    deviceInfo.DataStream.KillWait();
                    try
                    {
                        deviceInfo.DataStream.StopAcquisition(AcquisitionStopMode.Default);
                        deviceInfo.DataStream.Flush(DataStreamFlushMode.DiscardAll);
                    }
                    catch (Exception e) 
                    {
                        Debug.WriteLine("-- [BackEnd] Exception: " + e.Message);
                    }

                    foreach (var buffer in deviceInfo.DataStream.AnnouncedBuffers())
                    {
                        deviceInfo.DataStream.RevokeBuffer(buffer);
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
                deviceInfo.NodeMap?.FindNode<IntegerNode>("TLParamsLocked").SetValue(0);
            }
            catch (Exception e)
            {
                Debug.WriteLine("--- [BackEnd] Exception: " + e.Message);
            }
            
            // Close peak library
            Library.Close();
        }
    }
}
