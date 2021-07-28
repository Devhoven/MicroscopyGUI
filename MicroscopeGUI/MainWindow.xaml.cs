using uEye;
using System;
using System.IO;
using System.Text;
using uEye.Defines;
using System.Windows;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using MicroscopeGUI.UIElements.Steps;
using Brushes = System.Windows.Media.Brushes;
using Image = System.Windows.Controls.Image;
using Button = System.Windows.Controls.Button;
using uEye.Types;

namespace MicroscopeGUI
{
    public partial class UI : Window
    {
        public static Camera Cam;
        public static Image CurrentFrame;
        public static CustomShader FrameEffects;
        public static Dispatcher CurrentDispatcher;

        Thread WorkerThread;
        int[] MemoryIDs;

        ConfigStepCon ConfigCon;
        AnalysisStepCon AnalysisCon;

        HistogramWindow HistogramPopup;

        public UI()
        {
            InitializeComponent();

            InitializeCam();

            ConfigCon = new ConfigStepCon(ToolCon);
            AnalysisCon = new AnalysisStepCon(ToolCon);
            SetVisibillity(ConfigCon, ConfigConBtn);

            CurrentFrame = CurrentFrameCon;
            CurrentDispatcher = Dispatcher;
            FrameEffects = EffectShader;
            
            Closing += GUIClosing;

            StartCapture();
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
            MemoryIDs = new int[(int)Framerate];
            StatusRet = CreateRingBuffer();
            
            //Initialization failed, showing the error screen
            if (StatusRet != Status.SUCCESS)
            {
                CurrentFrameCon.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/NoCam.png"));
                ImageQueue.StopRunning = true;
            }
        }

        private Status CreateRingBuffer()
        {
            Status StatusRet;
            for (int i = 0; i < MemoryIDs.Length; i++)
            {
                StatusRet = Cam.Memory.Allocate(out int MemID, false);
                if (StatusRet == Status.Success)
                    Cam.Memory.Sequence.Add(MemID);
                MemoryIDs[i] = MemID;
            }
            StatusRet = Cam.Memory.Sequence.InitImageQueue();
            return StatusRet;
        }

        void StartCapture()
        {
            // Starting the live feed and the image queue thread
            Cam.Acquisition.Capture();
            WorkerThread.Start();
        }

        private void CloseCamera()
        {
            ImageQueue.StopRunning = true;
            WorkerThread.Join();
            // So, if the cam crashed or got pulled out in the process, the programm will still close correctly
            if (ImageQueue.CurrentCamStatus == Status.SUCCESS)
                Cam.Exit();
        }

        #region GUIEvents
        private void GUIClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!(HistogramPopup is null))
                HistogramPopup.Close();
            CloseCamera();
        }

        private void ChangeDirClick(object sender, RoutedEventArgs e) =>
            ImgGallery.UpdatePath();

        private void ConfigBtnClick(object sender, RoutedEventArgs e) =>
            SetVisibillity(ConfigCon, ConfigConBtn);

        private void AnalysisBtnClick(object sender, RoutedEventArgs e) =>
            SetVisibillity(AnalysisCon, AnalysisConBtn);

        void SetVisibillity(StepCon Con, Button Btn)
        {
            ConfigCon.Visibility = Visibility.Hidden;
            AnalysisCon.Visibility = Visibility.Hidden;

            ConfigConBtn.Background = Brushes.Transparent;
            AnalysisConBtn.Background = Brushes.Transparent;

            Con.Visibility = Visibility.Visible;
            Btn.Background = Brushes.LightSkyBlue;
        }

        private void HistClick(object sender, RoutedEventArgs e)
        {
            HistogramPopup = new HistogramWindow();
            HistogramPopup.Show();
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog SaveDialog = new SaveFileDialog();
            SaveDialog.Title = "Save file";
            SaveDialog.Filter = "Png|*.png";

            if (SaveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ImageQueue.CurrentFrameBitmap.Save(SaveDialog.FileName);
            }
        }

        private void ConfigSaveClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog SaveDialog = new SaveFileDialog();
            SaveDialog.Title = "Save file";
            SaveDialog.Filter = "Configfile|*.xml";

            if (SaveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                File.WriteAllText(SaveDialog.FileName, Control.GetXMLString(), Encoding.UTF8);
            }
        }

        private void ConfigLoadClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog OpenDialog = new OpenFileDialog();
            OpenDialog.Title = "Choose the config file";
            OpenDialog.Filter = "Configfile|*.xml";

            if (OpenDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Control.LoadXML(File.ReadAllText(OpenDialog.FileName, Encoding.UTF8));
            }
        }
        
        // Closes all the stuff the camera set up (Except the ring buffer) and initializes it again
        private void ReloadCamClick(object sender, RoutedEventArgs e)
        {
            // Closes the thread and joins it to the current
            ImageQueue.Mode = ImageQueue.ImgQueueMode.Frozen;
            ImageQueue.StopRunning = true;
            WorkerThread.Join();

            // Closes the camera
            Cam.Exit();

            // Initializes a new thread and a new camera
            InitializeCam();

            ImageQueue.Mode = ImageQueue.ImgQueueMode.Live;
            ImageQueue.StopRunning = false;

            StartCapture();

            // Reloading all of the control elements
            Control.RemoveAllControls();
            ToolCon.Children.Remove(ConfigCon);
            ToolCon.Children.Remove(AnalysisCon);
            ConfigCon = new ConfigStepCon(ToolCon);
            AnalysisCon = new AnalysisStepCon(ToolCon);
            SetVisibillity(ConfigCon, ConfigConBtn);
        }

        #endregion
    }
}