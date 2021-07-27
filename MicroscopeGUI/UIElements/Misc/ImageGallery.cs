using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System.Drawing;
using Image = System.Windows.Controls.Image;

namespace MicroscopeGUI
{
    class ImageGallery : StackPanel
    {
        static Thickness ImgBoxMargin = new Thickness()
        {
            Bottom = 5
        };

        // standard path 
        public string StandardPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop); 

        public ImageGallery()
        {
            string[] FilePaths = System.IO.Directory.GetFiles(StandardPath);
            LoadImagesFromFolder(FilePaths);
        }

        public void UpdatePath()
        {
            VistaFolderBrowserDialog Dialog = new VistaFolderBrowserDialog();
            if (Dialog.ShowDialog().GetValueOrDefault())
            {
                LoadImagesFromFolder(System.IO.Directory.GetFiles(Dialog.SelectedPath));
            }
        }

        private void LoadImagesFromFolder(string[] FilePaths)
        {
            Children.Clear();
            foreach (string Path in FilePaths)
            {
                if (Path.EndsWith(".png"))
                {
                    BitmapImage BmpImg = new BitmapImage();
                    BmpImg.BeginInit();
                    BmpImg.CacheOption = BitmapCacheOption.None;
                    BmpImg.UriSource = new Uri(Path);
                    BmpImg.EndInit();
                    Image NewImg = new Image()
                    {
                        Source = BmpImg,
                        Width = 150,
                        Height = 150,
                        Margin = ImgBoxMargin,
                        ToolTip = Path.Substring(Path.LastIndexOf("\\") + 1)
                    };
                    
                    Children.Add(NewImg);

                    NewImg.MouseLeftButtonDown += OnImageClick;
                }
            }

            // Clears up the unmanaged memory
            // Only fully clears up the images after the third time xD
            // But it is fine like this, since there is no memory leak now
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        void OnImageClick(object o, MouseButtonEventArgs e)
        {
            ImageQueue.Mode = ImageQueue.ImgQueueMode.ViewingAnotherImage;
            UI.CurrentFrame.Source = ((Image)o).Source;
        }
    }
}
