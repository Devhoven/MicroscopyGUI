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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using uEye;
using uEye.Defines;
using Image = System.Windows.Controls.Image;
using MicroscopeGUI.UIElements.Steps;
using uEye.Types;
using System.IO;
using System.Windows.Forms;

namespace MicroscopeGUI
{
    public partial class UI : Window
    {
        public static Camera Cam;
        public static Image CurrentFrame;
        public static Dispatcher CurrentDispatcher;

        Thread WorkerThread;

        ConfigStepCon ConfigCon;
        LocateStepCon LocateCon;
        AnalysisStepCon AnalysisCon;

        HistogramWindow HistogramPopup;

        public UI()
        {
            InitializeComponent();

            InitializeCam();

            ConfigCon = new ConfigStepCon(ToolCon);
            LocateCon = new LocateStepCon(ToolCon);
            AnalysisCon = new AnalysisStepCon(ToolCon);
            SetVisibillity(ConfigCon);

            CurrentFrame = CurrentFrameCon;
            CurrentDispatcher = Dispatcher;

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
            SetVisibillity(ConfigCon);

        private void LocateBtnClick(object sender, RoutedEventArgs e) =>
            SetVisibillity(LocateCon);

        private void AnalysisBtnClick(object sender, RoutedEventArgs e) =>
            SetVisibillity(AnalysisCon);

        void SetVisibillity(StepCon Con)
        {
            ConfigCon.Visibility = Visibility.Hidden;
            LocateCon.Visibility = Visibility.Hidden;
            AnalysisCon.Visibility = Visibility.Hidden;

            Con.Visibility = Visibility.Visible;
        }

        private void HistClick(object sender, RoutedEventArgs e)
        {
            HistogramPopup = new HistogramWindow();
            HistogramPopup.Show();
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)UI.CurrentFrame.Source.Width,
                                                                           (int)UI.CurrentFrame.Source.Height,
                                                                           100, 100, PixelFormats.Default);
            renderTargetBitmap.Render(UI.CurrentFrame);
            JpegBitmapEncoder jpegBitmapEncoder = new JpegBitmapEncoder();
            jpegBitmapEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));


            SaveFileDialog _saveFile = new SaveFileDialog();
            _saveFile.Title = "Save file";
            _saveFile.Filter = "Jpeg|*.jpeg";

            if (_saveFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                using (FileStream fileStream = new FileStream(_saveFile.FileName, FileMode.Create))
                {
                    jpegBitmapEncoder.Save(fileStream);
                    fileStream.Flush();
                    fileStream.Close();
                }
            }
        }
    }
}
