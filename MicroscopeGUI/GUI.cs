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
        public static PictureBox Display;

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

            // Setting the maximum size of the display, so the image won't get stretched
            Display = LiveImgCon;
            Camera.Size.AOI.Get(out Rectangle CameraRect);
            LiveImgCon.MaximumSize = CameraRect.Size;
            
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
            
            Tools = new StepCon[] { new ConfigStepCon(), new LocateStepCon(), new ProcessStepCon(), new AnalysisStepCon() };
            CurrentToolCon.Controls.Add(Tools[0]);
            SetLabelText();
        }

        private void NextBtnClick(object sender, EventArgs e)
        {
            CurrentToolCon.Controls.Remove(Tools[CurrentTool]);
            CurrentTool = Math.Min(CurrentTool + 1, Tools.Length - 1);
            CurrentToolCon.Controls.Add(Tools[CurrentTool], 0, 2);

            SetLabelText();
        }

        void SetLabelText()
        {
            if (CurrentTool - 1 >= 0)
                PreviousStepLabel.Text = Tools[CurrentTool - 1].Name;
            else
                PreviousStepLabel.Text = "";

            if (CurrentTool + 1 <= Tools.Length - 1)
                NextStepLabel.Text = Tools[CurrentTool + 1].Name;
            else
                NextStepLabel.Text = "";

            StepLabel.Text = Tools[CurrentTool].Name;
        }

        private void BackBtnClick(object sender, EventArgs e)
        {
            CurrentToolCon.Controls.Remove(Tools[CurrentTool]);
            CurrentTool = Math.Max(CurrentTool - 1, 0);
            CurrentToolCon.Controls.Add(Tools[CurrentTool], 0, 2);

            SetLabelText();
        }

        private void GUIClosing(object sender, FormClosingEventArgs e)
        {
            ImageQueue.StopRunning = true;
            WorkerThread.Join();
            Camera.Exit();
        }
    }
}
