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
using System.IO;
using Brushes = System.Windows.Media.Brushes;

namespace MicroscopeGUI
{
    partial class ImageGallery : StackPanel
    {
        static Thickness ImgBoxMargin = new Thickness()
        {
            Bottom = 5
        };

        FileSystemWatcher Watcher;

        public ImageGallery()
        {
            string Path = RegistryManager.GetStrVal("ImgGalleryPath");
            Settings.ImgGalleryPath = Path;
            if (Path != string.Empty)
            {
                string[] FilePaths = Directory.GetFiles(Path);
                LoadImagesFromFolder(FilePaths);

                InitializeWatcher(Path);
            }
        }

        public void UpdatePath()
        {
            VistaFolderBrowserDialog Dialog = new VistaFolderBrowserDialog();
            if (Dialog.ShowDialog().GetValueOrDefault())
            {
                UpdatePath(Dialog.SelectedPath);
            }
        }

        public void UpdatePath(string NewPath)
        {
            LoadImagesFromFolder(Directory.GetFiles(NewPath));
            RegistryManager.SetValue("ImgGalleryPath", NewPath);
            Settings.ImgGalleryPath = NewPath;
            UserInfo.SetInfo("Set the path to " + Settings.ImgGalleryPath);
        }

        private void LoadImagesFromFolder(string[] FilePaths)
        {
            Children.Clear();
            foreach (string Path in FilePaths)
            {
                if (Path.ToLower().EndsWith(".png"))
                {
                    Image NewImg = GetImage(Path);
                    
                    AddImageContextMenu(NewImg);

                    NewImg.MouseLeftButtonDown += OnImageClick;
                    NewImg.Cursor = Cursors.Hand;

                    Children.Add(NewImg);
                }
            }

            // Clears up the unmanaged memory
            // Only fully clears up the images after the third time xD
            // But it is fine like this, since there is no memory leak now
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        Image GetImage(string Path)
        {
            BitmapImage BmpImg = new BitmapImage();
            BmpImg.BeginInit();
            BmpImg.CacheOption = BitmapCacheOption.OnLoad;
            BmpImg.UriSource = new Uri(Path);
            BmpImg.EndInit();
            return new Image()
            {
                Source = BmpImg,
                Width = 180,
                Height = 180,
                Margin = ImgBoxMargin,
                ToolTip = Path.Substring(Path.LastIndexOf("\\") + 1)
            };
        }

        void AddImageContextMenu(Image NewImg)
        {
            NewImg.ContextMenu = new ContextMenu();
            MenuItem ValueEditItem = new MenuItem()
            {
                Header = "Edit Metadata",
                Icon = new Image
                {
                    Source = new BitmapImage(new Uri("pack://application:,,,/Assets/Icons/Edit.png"))
                }
            };
            MenuItem DeleteImageItem = new MenuItem()
            {
                Header = "Delete Image",
                Icon = new Image
                {
                    Source = new BitmapImage(new Uri("pack://application:,,,/Assets/Icons/Delete.png"))
                }
            };
            ValueEditItem.Click += MetadataViewClick;
            DeleteImageItem.Click += DeleteImageClick;
            NewImg.ContextMenu.Items.Add(ValueEditItem);
            NewImg.ContextMenu.Items.Add(DeleteImageItem);
        }

        private void MetadataViewClick(object sender, RoutedEventArgs e)
        {
            MenuItem Sender = sender as MenuItem;
            Image OriginalSource = (Sender.Parent as ContextMenu).PlacementTarget as Image;
            string OriginalPath = (OriginalSource.Source as BitmapImage).UriSource.OriginalString;

            using (FileStream Stream = new FileStream(OriginalPath, FileMode.Open, FileAccess.ReadWrite))
            {
                MetaDataWindow MetadataPopup = new MetaDataWindow(OriginalPath, MetadataEditor.GetValuePairs(Stream));
                MetadataPopup.Owner = Application.Current.MainWindow;
                MetadataPopup.Show();
            }
        }

        private void DeleteImageClick(object sender, RoutedEventArgs e)
        {
            MenuItem Sender = sender as MenuItem;
            Image OriginalSource = (Sender.Parent as ContextMenu).PlacementTarget as Image;
            string OriginalPath = (OriginalSource.Source as BitmapImage).UriSource.OriginalString;

            File.Delete(OriginalPath);

            Children.Remove(OriginalSource);
        }

        void OnImageClick(object o, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                ImageQueue.Mode = ImageQueue.ImgQueueMode.ViewingAnotherImage;
                UI.CurrentFrame.Source = ((Image)o).Source;
                UI Window = Application.Current.MainWindow as UI;
                Window.LiveFeedBtn.Background = Brushes.Transparent;
                Window.FreezeCamBtn.Background = Brushes.LightSkyBlue;
            }
        }
    }

    // Only handels the FileSystemWatcher for the image gallery
    partial class ImageGallery : StackPanel
    {
        // Initializes the FileSystemWatcher 
        void InitializeWatcher(string Path)
        {
            Watcher = new FileSystemWatcher(Path);

            Watcher.Created += Watcher_Changed;
            Watcher.Deleted += Watcher_Changed;
            Watcher.Renamed += Watcher_Changed;

            Watcher.Filter = "*.png";
            Watcher.IncludeSubdirectories = false;
            Watcher.EnableRaisingEvents = true;
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            string FolderPath = e.FullPath.Substring(0, e.FullPath.LastIndexOf("\\"));
            UI.CurrentDispatcher.Invoke(() => LoadImagesFromFolder(Directory.GetFiles(FolderPath)));
        }
    }
}
