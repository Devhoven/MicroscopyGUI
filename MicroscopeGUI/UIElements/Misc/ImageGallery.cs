using System;
using System.IO;
using System.Drawing;
using System.Windows;
using Ookii.Dialogs.Wpf;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Image = System.Windows.Controls.Image;
using Brushes = System.Windows.Media.Brushes;
using System.Drawing.Drawing2D;

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

        // Shows a dialog where the user can select a folder
        public void UpdatePath()
        {
            VistaFolderBrowserDialog Dialog = new VistaFolderBrowserDialog();
            if (Dialog.ShowDialog().GetValueOrDefault())
            {
                UpdatePath(Dialog.SelectedPath);
            }
        }

        // Updates the path in the settings and the registry
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

        // Returns an Image-control instance which holds an 180 * 180 pixels image
        Image GetImage(string Path)
        {
            using (MemoryStream Stream = new MemoryStream())
            {
                // Loading the file and resizing it to 180 * 180 pixels
                System.Drawing.Image Img = System.Drawing.Image.FromFile(Path).Resize(180, 180);
                // Getting a stream from the resized image
                Img.Save(Stream, System.Drawing.Imaging.ImageFormat.Png);
                Stream.Position = 0;

                // Loading the BitmapImage from which we can construct the Image-control
                BitmapImage BmpImg = new BitmapImage();
                BmpImg.BeginInit();
                BmpImg.CacheOption = BitmapCacheOption.OnLoad;
                BmpImg.StreamSource = Stream;
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
        }

        // Adds the options for editing the metadata and deleting the image to the Image-control
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

        // Shows the metadata from the image, so the user can edit it
        private void MetadataViewClick(object sender, RoutedEventArgs e)
        {
            MenuItem Sender = sender as MenuItem;
            Image OriginalSource = (Sender.Parent as ContextMenu).PlacementTarget as Image;
            // The ToolTip is the name of the image 
            string OriginalPath = Settings.ImgGalleryPath + "\\" + OriginalSource.ToolTip;

            using (FileStream Stream = new FileStream(OriginalPath, FileMode.Open, FileAccess.ReadWrite))
            {
                MetaDataWindow MetadataPopup = new MetaDataWindow(OriginalPath, MetadataEditor.GetValuePairs(Stream));
                MetadataPopup.Owner = Application.Current.MainWindow;
                MetadataPopup.Show();
            }
        }

        // Deletes the image 
        private void DeleteImageClick(object sender, RoutedEventArgs e)
        {
            MenuItem Sender = sender as MenuItem;
            Image OriginalSource = (Sender.Parent as ContextMenu).PlacementTarget as Image;
            // The ToolTip is the name of the image 
            string OriginalPath = Settings.ImgGalleryPath + "\\" + OriginalSource.ToolTip;

            File.Delete(OriginalPath);

            Children.Remove(OriginalSource);
        }

        void OnImageClick(object o, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                // Loading the image from the file, since the one in the image gallery is downsized
                BitmapImage BmpImg = new BitmapImage();
                BmpImg.BeginInit();
                BmpImg.CacheOption = BitmapCacheOption.OnLoad;
                BmpImg.UriSource = new Uri(Settings.ImgGalleryPath + "\\" + (o as Image).ToolTip);
                BmpImg.EndInit();

                UI.CurrentFrame.Source = BmpImg;
                // Setting the color of the buttons, so the user knows, that the image was "frozen"
                UI Window = Application.Current.MainWindow as UI;
                Window.LiveFeedBtn.Background = Brushes.Transparent;
                Window.FreezeCamBtn.Background = Brushes.LightSkyBlue;
                // And "freezing" the image
                ImageQueue.Mode = ImageQueue.ImgQueueMode.ViewingAnotherImage;
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

            // So if anything changes the FileSystemWatcher listens
            Watcher.Created += Watcher_Changed;
            Watcher.Deleted += Watcher_Changed;
            Watcher.Renamed += Watcher_Changed;

            // Only fires events if a .png file changes
            Watcher.Filter = "*.png";
            Watcher.IncludeSubdirectories = false;
            Watcher.EnableRaisingEvents = true;
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            // Reloading the image gallery
            UI.CurrentDispatcher.Invoke(() => LoadImagesFromFolder(Directory.GetFiles(Settings.ImgGalleryPath)));
        }
    }
}
