using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using uEye;
using uEye.Defines;

namespace MicroscopeGUI
{
    public partial class UI : Window
    {
        public static Camera Cam;

        Thread WorkerThread;

        public UI()
        {
            InitializeCam();

            InitializeComponent();

            Closing += GUIClosing;
        }


        void InitializeCam()
        {
            // For debugging the camera
            Status StatusRet;

            // Camera initialization
            Cam = new Camera();
            StatusRet = Cam.Init();

            // Initializing the thread, which runs the image queue
            WorkerThread = new Thread(ImageQueue.Run);

            // Allocating image buffers for 1 second of an image queue
            Cam.Timing.Framerate.Get(out double Framerate);
            for (int i = 0; i < (int)Framerate; i++)
            {
                StatusRet = Cam.Memory.Allocate(out int MemID, false);
                if (StatusRet == Status.Success)
                    Cam.Memory.Sequence.Add(MemID);
            }
            StatusRet = Cam.Memory.Sequence.InitImageQueue();

            // Starting the live feed and the image queue thread
            StatusRet = Cam.Acquisition.Capture();
            WorkerThread.Start();
        }
        private void GUIClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ImageQueue.StopRunning = true;
            WorkerThread.Join();
            Cam.Exit();
        }
    }
}
