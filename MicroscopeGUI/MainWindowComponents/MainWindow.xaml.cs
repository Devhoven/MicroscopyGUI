using System;
using System.IO;
using System.Windows;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using Image = System.Windows.Controls.Image;
using Brushes = System.Windows.Media.Brushes;
using MessageBox = System.Windows.MessageBox;
using MicroscopeGUI.MainWindowComponents.ImageViewer.Shader.Classes;

namespace MicroscopeGUI
{
    public partial class UI : Window
    {
        public static Image CurrentFrame;
        public static CustomShader FrameEffects;
        public static Dispatcher CurrentDispatcher;

        public static HistogramControl HistogramControl;

        readonly MemoryStream BmpMemory;

        MetaDataWindow MetadataPopup;
        KeybindWindow KeybindPopup;

        public UI()
        {
            InitializeComponent();

            InitializeUIComponents();

            CurrentFrame = CurrentFrameCon;
            CurrentDispatcher = Dispatcher;
            FrameEffects = EffectShader;

            PreviewKeyDown += UIKeyDown;
            Closing += GUIClosing;

            CamControl.ImageReceived += CamImageReceived;

            CamControl.Start();

            BmpMemory = new MemoryStream();

            ConfigCon.Children.Add(CamControl.GetControlCon());
        }

        private void CamImageReceived(Bitmap bitmap)
        {
            bitmap.Save(BmpMemory, ImageFormat.Bmp);
            BmpMemory.Position = 0;
            BitmapImage bitmapimage = new BitmapImage();
            bitmapimage.BeginInit();
            bitmapimage.StreamSource = BmpMemory;
            bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapimage.EndInit();
            bitmapimage.Freeze();

            CurrentFrame.Dispatcher.BeginInvoke(new Action<UI>(delegate { CurrentFrame.Source = bitmapimage; }), new object[] { this });
        }

        // Restarts the camera and reloads the UI controls for the camera
        void ReloadCamera()
        {
            UserInfo.SetInfo("Reloading the cam...");

            ConfigCon.Children.Clear();

            CamControl.Stop();

            CamControl.Initialize();
            CamControl.Start();

            ConfigCon.Children.Add(CamControl.GetControlCon());
        }

        void InitializeUIComponents()
        {
            HistogramControl = new HistogramControl(HistogramPlot);

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
            if (!(KeybindPopup is null))
                KeybindPopup.Close();

            CamControl.Stop();
        }
    }
}