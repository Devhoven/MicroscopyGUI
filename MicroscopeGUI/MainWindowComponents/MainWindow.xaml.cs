using System;
using System.IO;
using System.Windows;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using Image = System.Windows.Controls.Image;
using Brushes = System.Windows.Media.Brushes;
using MicroscopeGUI.MetadataWindowComponents;
using MicroscopeGUI.IDSPeak;
using System.Windows.Input;

namespace MicroscopeGUI
{
    public partial class UI : Window
    {
        public static Image CurrentFrame;
        public static CustomShader FrameEffects;
        public static Dispatcher CurrentDispatcher;
        public static ImageGallery CurrentGallery;

        public static HistogramControl HistogramControl;

        readonly MemoryStream BmpMemory;

        MetadataWindow MetadataPopup;
        KeybindWindow KeybindPopup;

        public UI()
        {
            InitializeComponent();

            CurrentFrame = CurrentFrameCon;
            CurrentDispatcher = Dispatcher;
            FrameEffects = EffectShader;
            CurrentGallery = ImgGallery;

            BmpMemory = new MemoryStream();

            DeviceSelector.InitializeLibrary();

            InitUIComponents();
        }

        // Restarts the camera and reloads the UI controls for the camera
        void ReloadCamera()
        {
            UserInfo.SetInfo("Reloading the camera...");

            ConfigCon.Children.Clear();

            CamControl.Stop();

            if (!CamControl.Start())
            {
                UserInfo.SetErrorInfo("Could not reload the camera");
                return;
            }

            UserInfo.SetInfo("Finished reloading the camera");

            ConfigCon.Children.Add(CamControl.GetControlCon());
        }

        void InitUIComponents()
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

            CamControl.ImageReceived += CamImageReceived;
            if (CamControl.Start())
                ConfigCon.Children.Add(CamControl.GetControlCon());
            else
                UserInfo.SetErrorInfo("Could not start the camera");
        }

        private void CamImageReceived(Bitmap bitmap)
        {
            // Conversion from a Bitmap into an ImageSource
            bitmap.Save(BmpMemory, ImageFormat.Bmp);
            BmpMemory.Position = 0;
            BitmapImage bitmapimage = new BitmapImage();
            bitmapimage.BeginInit();
            bitmapimage.StreamSource = BmpMemory;
            bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapimage.EndInit();
            bitmapimage.Freeze();

            // Calling the dispatcher here, since this method is always going to be called from another thread
            if (!CamControl.IsFrozen)
                CurrentFrame.Dispatcher.BeginInvoke(() => CurrentFrame.Source = bitmapimage);
        }

        protected override void OnClosed(EventArgs e)
        {
            MetadataPopup?.Close();
            KeybindPopup?.Close();

            CamControl.Stop();
            DeviceSelector.CloseLibrary();
        }
    }
}