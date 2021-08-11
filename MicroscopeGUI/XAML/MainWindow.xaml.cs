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
using Button = System.Windows.Controls.Button;

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

            InitializeUIComponents();

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

        private void GUIClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!(HistogramPopup is null))
                HistogramPopup.Close();
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

    // Class for all of the ui element events
    public partial class UI : Window
    {
        // Changes the directory of the image gallery
        private void ChangeDirClick(object sender, RoutedEventArgs e) =>
            ImgGallery.UpdatePath();

        // Makes the config step visible
        private void ConfigBtnClick(object sender, RoutedEventArgs e)
        {
            SetVisibillity(ConfigCon, ConfigConBtn);
            RegistryManager.SetValue("CurrentConfigStep", 0);
        }

        // Makes the analysis step visible
        private void AnalysisBtnClick(object sender, RoutedEventArgs e)
        {
            SetVisibillity(AnalysisCon, AnalysisConBtn);
            RegistryManager.SetValue("CurrentConfigStep", 1);
        }

        // Sets the visibillity of the containers and changes the color of the buttons
        void SetVisibillity(StepCon Con, Button Btn)
        {
            ConfigCon.Visibility = Visibility.Hidden;
            AnalysisCon.Visibility = Visibility.Hidden;

            ConfigConBtn.Background = Brushes.Transparent;
            AnalysisConBtn.Background = Brushes.Transparent;

            Con.Visibility = Visibility.Visible;
            Btn.Background = Brushes.LightSkyBlue;
        }

        // Opens the histogram window
        private void HistClick(object sender, RoutedEventArgs e)
        {
            HistogramPopup = new HistogramWindow();
            HistogramPopup.Show();
        }

        // Opens the settings
        private void SettingsClick(object sender, RoutedEventArgs e)
        {
            SettingsWindow Settings = new SettingsWindow();
            Settings.Owner = this;
            Settings.Show();
        }

        // Saves the current frame
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

        // Saves a config
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

        // Loads a config
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
     
        private void MeasureBtnClick(object sender, RoutedEventArgs e)
        {
            if (MeasureBtn.Background == Brushes.LightBlue)
                MeasureBtn.Background = Brushes.Transparent;
            else
                MeasureBtn.Background = Brushes.LightBlue;

            ZoomDisplay.ToggleMode();
        }

        private void ConfigConToggle(object sender, RoutedEventArgs e) =>
            ToggleItemVisibillity((MenuItem)sender, ToolCon, "ConfigConActivated");

        private void ImgGalleryToggle(object sender, RoutedEventArgs e) =>
            ToggleItemVisibillity((MenuItem)sender, ImgGalleryCon, "ImgGalleryActivated");

        // Toggles the visibillity of a UI Elements and with it the color of the specified MenuItem
        private void ToggleItemVisibillity(MenuItem Sender, UIElement Element, string ValName)
        {
            if (Element.Visibility == Visibility.Visible)
            {
                Element.Visibility = Visibility.Collapsed;
                Sender.Background = Brushes.Transparent;
                RegistryManager.SetValue(ValName, false);
            }
            else
            {
                Element.Visibility = Visibility.Visible;
                Sender.Background = Brushes.LightBlue;
                RegistryManager.SetValue(ValName, true);
            }
        }
    }
}