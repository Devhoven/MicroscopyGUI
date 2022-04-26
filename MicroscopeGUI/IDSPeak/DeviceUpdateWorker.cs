using peak;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace MicroscopeGUI.IDSPeak
{
    [Obsolete ("This class can't be used yet, since there are still a few bugs in the IDS Peak Library")]
    internal class DeviceUpdateWorker
    {
        bool Running = false;

        Thread UpdateThread;

        List<string> LostDevices;

        public void Start()
        {
            LostDevices = new List<string>();

            DeviceManager.Instance().DeviceFoundEvent += DeviceManager_DeviceFound;
            DeviceManager.Instance().DeviceLostEvent += DeviceManager_DeviceLost;

            UpdateThread = new Thread(new ThreadStart(Loop));
            UpdateThread.Start();
        }

        void Loop()
        {
            Running = true;

            while (Running)
            {
                try
                {
                    var deviceManager = DeviceManager.Instance();
                    deviceManager.Update();
                }
                catch (Exception)
                { }
                Thread.Sleep(500);
            }
        }

        public void Stop()
        {
            Running = false;

            UpdateThread?.Join();
        }

        private void DeviceManager_DeviceFound(object sender, peak.core.DeviceDescriptor e)
        {

        }

        private void DeviceManager_DeviceLost(object sender, string deviceID)
        {
            LostDevices.Add(deviceID);
            UI.CurrentDispatcher.BeginInvoke(() => UserInfo.SetErrorInfo("Lost Device"), DispatcherPriority.Normal);
        }
    }
}
