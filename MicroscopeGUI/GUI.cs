using System;
using uEye;
using uEye.Defines;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;


namespace MicroscopeGUI
{
    public partial class GUI : Form
    {
        public static Camera Camera;
        public static PictureViewer Display;

        Thread WorkerThread;

        StepCon[] Tools;
        int CurrentTool;

        public GUI()
        {
            InitializeComponent();

            Status StatusRet;

            // Camera initialization
            Camera = new Camera();
            StatusRet = Camera.Init();

            // GUI Setup
            Display = new PictureViewer();
            MainLayout.Controls.Add(Display, 1, 0);
            
            //Display = LiveImgCon;
            ExitMenuItem.Click += new EventHandler(delegate (object o, EventArgs a)
            {
                Close();
            });

            // Initializing the thread, which runs the image queue
            WorkerThread = new Thread(ImageQueue.Run);

            // Allocating image buffers for 1 second of an image queue
            Camera.Timing.Framerate.Get(out double Framerate);
            for (int i = 0; i < (int)Framerate; i++)
            {
                StatusRet = Camera.Memory.Allocate(out int MemID, false);
                if (StatusRet == Status.Success)
                    Camera.Memory.Sequence.Add(MemID);
            }
            StatusRet = Camera.Memory.Sequence.InitImageQueue();

            // Starting the live feed and the image queue thread
            StatusRet = Camera.Acquisition.Capture();
            WorkerThread.Start();
            
            Tools = new StepCon[] { new ConfigStepCon(), new LocateStepCon(), new AnalysisStepCon() };
            CurrentToolCon.Controls.Add(Tools[0]);
        }

        // Highlighting the different labels 
        private void ConfigStepLabel_Click(object sender, EventArgs e)
        {
            HighlightLabel(ConfigStepLabel, 0);
        }
        private void LocateStepLabel_Click(object sender, EventArgs e)
        {
            HighlightLabel(LocateStepLabel, 1);
        }
        private void AnalysisStepLabel_Click(object sender, EventArgs e)
        {
            HighlightLabel(AnalysisStepLabel, 2);
        }

        void HighlightLabel(Label CurrentLabel, int NextTool)
        {
            ConfigStepLabel.TextAlign = ContentAlignment.BottomCenter;
            LocateStepLabel.TextAlign = ContentAlignment.BottomCenter;
            AnalysisStepLabel.TextAlign = ContentAlignment.BottomCenter;
            CurrentLabel.TextAlign = ContentAlignment.MiddleCenter;
            CurrentToolCon.Controls.Remove(Tools[CurrentTool]);
            CurrentTool = NextTool;
            CurrentToolCon.Controls.Add(Tools[CurrentTool]);
        }

        private void GUIClosing(object sender, FormClosingEventArgs e)
        {
            ImageQueue.StopRunning = true;
            WorkerThread.Join();
            Camera.Exit();
        }
    }
}
