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

namespace MicroscopeGUI
{
    class ImageGallery : StackPanel
    {
        public ImageGallery()
        {
            VistaFolderBrowserDialog Dialog = new VistaFolderBrowserDialog();
            if (Dialog.ShowDialog().GetValueOrDefault())
            {
                string NewDir = Dialog.SelectedPath;
                // Returns all of the file paths
                string[] FilePaths = System.IO.Directory.GetFiles(NewDir);
                // loading images from images 
                LoadImagesFromFolder(FilePaths);
            }
        }

        private void LoadImagesFromFolder(string[] FilePaths)
        {
            Thickness ImgBoxMargin = new Thickness()
            {
                Bottom = 5
            };
            foreach (string Path in FilePaths)
            {
                if (Path.EndsWith(".png"))
                {
                    Children.Add(new System.Windows.Controls.Image() {
                        Source = new BitmapImage(new Uri(Path)),
                        Width = 150,
                        Height = 150,
                        Margin = ImgBoxMargin
                    });
                }
            }
        }
    }
}
