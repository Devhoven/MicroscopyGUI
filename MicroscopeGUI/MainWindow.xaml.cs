using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.Windows.Threading;
using uEye;
using uEye.Defines;
using Image = System.Windows.Controls.Image;

namespace MicroscopeGUI
{
    public partial class UI : Window
    {
        public static Camera Cam;
        public static Image CurrentFrame;
        public static Dispatcher CurrentDispatcher;

        Thread WorkerThread;

        double[] ValuesR;
        double[] ValuesG;
        double[] ValuesB;
        double[] ValuesX;

        public UI()
        {
            InitializeCam();

            InitializeComponent();

            CurrentFrame = Test;
            CurrentDispatcher = Dispatcher;

            Closing += GUIClosing;

            StartCapture();

            ValuesR = new double[255];
            ValuesG = new double[255];
            ValuesB = new double[255];
            ValuesX = new double[255];
            for (int i = 0; i < 255; i++)
                ValuesX[i] = i;

            ImageQueue.OnFrameChange += new EventHandler(delegate (object o, EventArgs e)
            {
                WpfPlot1.Plot.Clear();
                for (int i = 0; i < 255; i++)
                    ValuesR[i] = ImageQueue.Histogram[i];

                //for (int i = 256; i < 511; i++)
                //    ValuesG[i - 256] = ImageQueue.Histogram[i];

                //for (int i = 512; i < 767; i++)
                //    ValuesB[i - 512] = ImageQueue.Histogram[i];

                WpfPlot1.Plot.PlotBar(ValuesX, ValuesR, null, "Labelu", 0.8, 0, true, System.Drawing.Color.Red);
            });
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
        }

        void StartCapture()
        {
            // Starting the live feed and the image queue thread
            Cam.Acquisition.Capture();
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
