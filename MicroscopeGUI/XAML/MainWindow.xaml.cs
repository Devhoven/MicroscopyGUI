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
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

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

        HistogramWindow HistogramPopup;

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
            Cam.Information.SetEnableErrorReport(true);

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
                    CurrentFrameCon.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/NoCam.png"));
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
            if (!(HistogramPopup is null))
                HistogramPopup.Close();
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

    // Class for all of the ui element events
    public partial class UI : Window
    {
        // Changes the directory of the image gallery
        void ChangeDirClick(object sender, RoutedEventArgs e) =>
            ImgGallery.UpdatePath();

        // Makes the config step visible
        void ConfigBtnClick(object sender, RoutedEventArgs e)
        {
            SetVisibillity(ConfigCon, ConfigConBtn);
            RegistryManager.SetValue("CurrentConfigStep", 0);
        }

        // Makes the analysis step visible
        void AnalysisBtnClick(object sender, RoutedEventArgs e)
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
        void HistClick(object sender, RoutedEventArgs e)
        {
            HistogramPopup = new HistogramWindow();
            // So the window always stays on top of the main window
            HistogramPopup.Owner = this;
            HistogramPopup.Show();
        }

        // Opens the settings
        void SettingsClick(object sender, RoutedEventArgs e)
        {
            SettingsWindow Settings = new SettingsWindow();
            Settings.Owner = this;
            Settings.Show();
        }

        // Saves the current frame
        void SaveClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog SaveDialog = new SaveFileDialog();
            SaveDialog.Title = "Save file";
            SaveDialog.Filter = "Png|*.png";

            if (SaveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MetadataPopup = new MetaDataWindow(SaveDialog.FileName);
                MetadataPopup.Show();
            }
        }

        // Saves a config
        void ConfigSaveClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog SaveDialog = new SaveFileDialog();
            SaveDialog.Title = "Save file";
            SaveDialog.Filter = "Configfile|*.xml";

            if (SaveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                File.WriteAllText(SaveDialog.FileName, Control.GetXMLString(), Encoding.UTF8);
                UserInfo.SetInfo("Saved the current config to " + SaveDialog.FileName);
            }
            else
                UserInfo.SetInfo("Action aborted");
        }

        // Loads a config
        void ConfigLoadClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog OpenDialog = new OpenFileDialog();
            OpenDialog.Title = "Choose the config file";
            OpenDialog.Filter = "Configfile|*.xml";

            if (OpenDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Control.LoadXML(File.ReadAllText(OpenDialog.FileName, Encoding.UTF8));
                UserInfo.SetInfo("Loaded the config from " + OpenDialog.FileName);
            }
            else
                UserInfo.SetInfo("Action aborted");
        }

        // Closes all the stuff the camera set up (Except the ring buffer) and initializes it again
        void ReloadCamClick(object sender, RoutedEventArgs e)
        {
            UserInfo.SetInfo("Reloading the cam...");

            // Starts a new thread with the camera restarting logic inthere
            Thread RestartThread = new Thread(RestartCam);
            RestartThread.Start();
            // Tries to join the thread together with the normal one again
            // When this is done, the Join method waits for 10 seconds till the thread is finished
            // If it hasn't finished until then, the restarting is aborted
            if (!RestartThread.Join(TimeSpan.FromSeconds(10)))
            {
                RestartThread.Interrupt();
                UserInfo.SetInfo("Aborted reloading the cam, since it took too long ( > 10 seconds), check if everything is plugged in correctly");
            }
            else
            {
                // Reloading all of the control elements
                Control.RemoveAllControls();
                ToolCon.Children.Remove(ConfigCon);
                ToolCon.Children.Remove(AnalysisCon);
                ConfigCon = new ConfigStepCon(ToolCon);
                AnalysisCon = new AnalysisStepCon(ToolCon);
                SetVisibillity(ConfigCon, ConfigConBtn);

                UserInfo.SetInfo("Reloaded the cam");

                if (!(OldXMLConfig is null))
                {
                    MessageBoxResult Result = MessageBox.Show("There is a saved config from before the camera crashed\nDo you want to load it?", "Question", MessageBoxButton.YesNo);
                    if (Result == MessageBoxResult.Yes)
                    {
                        Control.LoadXML(OldXMLConfig);
                        UserInfo.SetInfo("Loaded the config");
                    }
                }
            }

            // Capsuled in a method, so I can put it in a seperate thread
            void RestartCam()
            {
                // Closes the thread and joins it to the current
                ImageQueue.Mode = ImageQueue.ImgQueueMode.Frozen;
                ImageQueue.StopRunning = true;
                WorkerThread.Join();

                // Closes the camera
                Cam.Exit();

                // Initializes a new thread and a new camera
                InitializeCam(false);

                ImageQueue.Mode = ImageQueue.ImgQueueMode.Live;
                ImageQueue.StopRunning = false;

                StartCapture();
            }
        }

        // Freezes the cam
        void FreezeCamClick(object sender, RoutedEventArgs e)
        {
            Cam.Acquisition.Freeze();
            ImageQueue.Mode = ImageQueue.ImgQueueMode.Frozen;

            UserInfo.SetInfo("Freezed the camera");
        }

        // Starts the camera live feed again
        void LiveFeedClick(object sender, RoutedEventArgs e)
        {
            Cam.Acquisition.Capture();
            ImageQueue.Mode = ImageQueue.ImgQueueMode.Live;

            UserInfo.SetInfo("Started the live feed");
        }

        void MeasureBtnClick(object sender, RoutedEventArgs e)
        {
            if (MeasureBtn.Background == Brushes.LightBlue)
            {
                UserInfo.SetInfo("You can now measure again");
                MeasureBtn.Background = Brushes.Transparent;
                ZoomDisplay.SetMeasureMode(ImageViewer.MeasureMode.Rectangle);
            }
            else
            {
                UserInfo.SetInfo("Draw a line to measure", Brushes.LightGreen, Brushes.Black);
                MeasureBtn.Background = Brushes.LightBlue;
                ZoomDisplay.SetMeasureMode(ImageViewer.MeasureMode.MeasureFactor);
            }
        }

        void ConfigConToggle(object sender, RoutedEventArgs e) =>
            ToggleItemVisibillity((MenuItem)sender, ToolCon, "ConfigConActivated");

        void ImgGalleryToggle(object sender, RoutedEventArgs e) =>
            ToggleItemVisibillity((MenuItem)sender, ImgGalleryCon, "ImgGalleryActivated");

        // Toggles the visibillity of a UI Elements and with it the color of the specified MenuItem
        void ToggleItemVisibillity(MenuItem Sender, UIElement Element, string ValName)
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