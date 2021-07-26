using uEye;
using System;
using System.Linq;
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
using System.Windows.Input;
using P = MicroscopeGUI.Properties.Settings;

namespace MicroscopeGUI
{
    public partial class UI : Window
    {
        public static Camera Cam;
        public static Image CurrentFrame;
        public static EffectShader FrameEffects;
        public static Dispatcher CurrentDispatcher;

        Thread WorkerThread;

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
            for (int i = 0; i < (int)Framerate; i++)
            {
                StatusRet = Cam.Memory.Allocate(out int MemID, false);
                if (StatusRet == Status.Success)
                    Cam.Memory.Sequence.Add(MemID);
            }

            StatusRet = Cam.Memory.Sequence.InitImageQueue();
            //Initialization failed, showing the error screen
            if (StatusRet != Status.SUCCESS)
            {
                CurrentFrameCon.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/NoCam.png"));
                ImageQueue.StopRunning = true;
            }
        }

        void StartCapture()
        {
            // Starting the live feed and the image queue thread
            Cam.Acquisition.Capture();
            WorkerThread.Start();
        }

        private void GUIClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!(HistogramPopup is null))
                HistogramPopup.Close();
            ImageQueue.StopRunning = true;
            WorkerThread.Join();
            // So, if the cam crashed or got pulled out in the process, the programm will still close correctly
            if (ImageQueue.CurrentCamStatus == Status.SUCCESS)
                Cam.Exit();
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
            // Auto Shutter
            P.Default.ShutterEnable = Cam.AutoFeatures.Software.Shutter.Enabled;
            // Auto WhiteBalance
            P.Default.WhiteBalanceEnable = Cam.AutoFeatures.Software.WhiteBalance.Enabled;
            // Auto Gain
            P.Default.GainEnable = Cam.AutoFeatures.Software.Gain.Enabled;
            // Framerate
            Cam.Timing.Framerate.GetFrameRateRange(out double FPSMin, out double FPSMax, out _);
            Cam.Timing.Framerate.Get(out double CurrentFPS);
            P.Default.FrameRateMax = FPSMax;
            P.Default.FrameRateMin = FPSMin;
            P.Default.CurrentFramerate = CurrentFPS;
            // Gamma
            Cam.Gamma.Software.GetRange(out int GammaMin, out int GammaMax, out _);
            Cam.Gamma.Software.GetDefault(out int DefaultVal);
            Cam.Gamma.Software.Get(out int GammaVal);
            P.Default.GammaMax = GammaMax;
            P.Default.GammaMin = GammaMin;
            P.Default.GammaDefault = DefaultVal;
            P.Default.GammaVal = GammaVal;
            // Brightness
            Cam.AutoFeatures.Software.Reference.Get(out uint Brightness);
            P.Default.Brightness = Brightness;
            // Master Gain
            Cam.Gain.Hardware.Factor.GetMaster(out int Master);
            P.Default.MasterGain = Master;
            // Color Temperature
            Cam.Color.Temperature.GetRange(out uint MinTemp, out uint MaxTemp, out _);
            Cam.Color.Temperature.GetDefault(out uint DefaultTemp);
            P.Default.ColorTempMax = MaxTemp;
            P.Default.ColorTempMin = MinTemp;
            P.Default.ColorTempDefault = DefaultTemp;
            // Saturation
            Cam.Saturation.Get(out int satVal);
            P.Default.Saturation = satVal;
            // Exposure
            Cam.Timing.Exposure.Get(out double CurrentExposure);
            Cam.Timing.Exposure.GetRange(out double MinExposure, out double MaxExposure, out _);
            P.Default.ExposureMax = MaxExposure;
            P.Default.ExposureMin = MinExposure;
            P.Default.CurrentExposure = CurrentExposure;

            // Saving Values in Settings.settings file
            P.Default.Save();

            System.Windows.MessageBox.Show("Config saved!", "Microscope GUI");
        }

        private void ConfigLoadClick(object sender, RoutedEventArgs e)
        {
            // Auto Shutter
            Cam.AutoFeatures.Software.Shutter.Enabled = P.Default.ShutterEnable;
            // Auto WhiteBalance
            Cam.AutoFeatures.Software.WhiteBalance.Enabled = P.Default.WhiteBalanceEnable;
            // Auto Gain
            Cam.AutoFeatures.Software.Gain.Enabled = P.Default.GainEnable;
            // Framerate
            Cam.Timing.Framerate.Set(P.Default.CurrentFramerate);
            // Gamma
            Cam.Gamma.Software.Set(P.Default.GammaVal);
            // Brightness
            Cam.AutoFeatures.Software.Reference.Set(P.Default.Brightness);
            // Master Gain
            Cam.Gain.Hardware.Factor.SetMaster(P.Default.MasterGain);
            // Color Temperature
            Cam.Color.Temperature.Set(P.Default.CurrentTemp);
            // Saturation
            Cam.Saturation.Set(P.Default.Saturation);
            // Exposure
            Cam.Timing.Exposure.Set(P.Default.CurrentExposure);

            System.Windows.MessageBox.Show("Config loaded!", "Microscope GUI");
        }
    }
}