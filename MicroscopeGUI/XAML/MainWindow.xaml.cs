using uEye;
using System;
using System.IO;
using System.Text;
using uEye.Defines;
using System.Windows;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using MicroscopeGUI.UIElements.Steps;
using Image = System.Windows.Controls.Image;
using Brushes = System.Windows.Media.Brushes;
using MessageBox = System.Windows.MessageBox;
using Button = System.Windows.Controls.Button;
using Application = System.Windows.Application;

namespace MicroscopeGUI
{
    public partial class UI : Window
    {
        public static Camera Cam;
        public static Image CurrentFrame;
        public static CustomShader FrameEffects;
        public static Dispatcher CurrentDispatcher;

        public static string OldXMLConfig;

        Thread WorkerThread;
        int[] MemoryIDs;

        ConfigStepCon ConfigCon;
        AnalysisStepCon AnalysisCon;

        MetaDataWindow MetadataPopup;

        public UI()
        {
            InitializeComponent();

            InitializeCam();

            InitializeUIComponents();

            CurrentFrame = CurrentFrameCon;
            CurrentDispatcher = Dispatcher;
            FrameEffects = EffectShader;
            
            Closing += GUIClosing;

            StartCapture();
        }

        // The bool is for the reloading cam feature
        void InitializeCam(bool SetErrorImage = true)
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
                if (SetErrorImage)
                {
                    CurrentFrameCon.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/NoCamConnected.png"));
                    UserInfo.SetErrorInfo("ERROR: " + Enum.GetName(typeof(Status), StatusRet) + "(" + (int)StatusRet + ")");
                }
                ImageQueue.StopRunning = true;
            }
        }

        Status CreateRingBuffer()
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

        void InitializeUIComponents()
        {
            ConfigCon = new ConfigStepCon(ToolCon);
            AnalysisCon = new AnalysisStepCon(ToolCon);
            new HistogramControl(HistogramPlot);
            int Selected = RegistryManager.GetIntVal("CurrentConfigStep");

            if (Selected == 1)
                SetVisibillity(AnalysisCon, AnalysisConBtn);
            else
                SetVisibillity(ConfigCon, ConfigConBtn);

            bool ConfigConActivated = RegistryManager.GetBoolVal("ConfigConActivated");
            if (!ConfigConActivated)
            {
                ToolCon.Visibility = Visibility.Collapsed;
                ConfigConToggleBtn.Background = Brushes.Transparent;
            }

            bool ImgGalleryActivated = RegistryManager.GetBoolVal("ImgGalleryActivated");
            if (!ImgGalleryActivated)
            {
                ImgGalleryCon.Visibility = Visibility.Collapsed;
                ImgGalleryToggleBtn.Background = Brushes.Transparent;
            }
        }

        void GUIClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!(MetadataPopup is null))
                MetadataPopup.Close();
            CloseCamera();
        }

        void CloseCamera()
        {
            ImageQueue.StopRunning = true;
            WorkerThread.Join();
            // So, if the cam crashed or got pulled out in the process, the programm will still close correctly
            if (ImageQueue.CurrentCamStatus == Status.SUCCESS)
                Cam.Exit();
        }

    }
}