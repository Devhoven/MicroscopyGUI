using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Image = System.Windows.Controls.Image;
using SharpImg = SixLabors.ImageSharp.Image;
using Brushes = System.Windows.Media.Brushes;
using DialogResult = System.Windows.Forms.DialogResult;
using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;
using MicroscopeGUI.MetadataWindowComponents;
using System.Collections.Generic;
using System.Windows.Threading;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp;
using System.Threading;

namespace MicroscopeGUI
{
    public partial class ImageGallery : StackPanel
    {
        static Thickness ImgBoxMargin = new Thickness()
        {
            Bottom = 5
        };

        Dictionary <string, Image> Images = new Dictionary<string, Image>();

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
            FolderBrowserDialog dialog = new FolderBrowserDialog()
            {
                InitialDirectory = Settings.ImgGalleryPath
            };

            if (dialog.ShowDialog() == DialogResult.OK)
                UpdatePath(dialog.SelectedPath);
        }

        // Updates the path in the settings and the registry
        public void UpdatePath(string newPath)
        {
            if (Settings.ImgGalleryPath == newPath)
                return;

            LoadImagesFromFolder(Directory.GetFiles(newPath));
            RegistryManager.SetValue("ImgGalleryPath", newPath);
            Settings.ImgGalleryPath = newPath;
            UserInfo.SetInfo("Set the path to " + Settings.ImgGalleryPath);
            InitializeWatcher(newPath);
        }

        private void LoadImagesFromFolder(string[] FilePaths)
        {
            Images.Clear();
            Children.Clear();
            foreach (string path in FilePaths)
            {
                if (path.ToLower().EndsWith(".png"))
                    AddImg(path);
            }

            // Clears up the unmanaged memory
            // Only fully clears up the images after the third time xD
            // But it is fine like this, since there is no memory leak now
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        void AddImg(string path)
        {
            Image newImg = GetImage(path);
            // The image could not be loaded
            if (newImg == null)
                return;

            AddImageContextMenu(newImg);

            newImg.MouseLeftButtonDown += OnImageClick;
            newImg.Cursor = Cursors.Hand;

            Children.Add(newImg);
            Images.Add(Path.GetFileName(path), newImg);
        }

        void RemoveImg(string name)
        {
            if (!Images.ContainsKey(name))
                return;

            Children.Remove(Images[name]);
            Images.Remove(name);
        }

        // Returns an Image-control instance which holds an 180 * 180 pixels image
        Image GetImage(string path)
        {
            // Waits until the file is freed
            // After 200ms it stops trying
            SpinWait.SpinUntil(() => IsFileReady(path), 200);
            if (!IsFileReady(path))
                return null;

            using (MemoryStream stream = new MemoryStream())
            {
                // Loading the file and resizing it to 180 * 180 pixels
                using (SharpImg img = SharpImg.Load(path, out IImageFormat format))
                {
                    img.Mutate(i => i.Resize(new ResizeOptions()
                    {
                        Mode = SixLabors.ImageSharp.Processing.ResizeMode.Max,
                        Size = new SixLabors.ImageSharp.Size(180, 180)
                    }));
                    // Getting a stream from the resized image
                    img.Save(stream, format);
                    stream.Position = 0;

                    // Loading the BitmapImage from which we can construct the Image-control
                    BitmapImage bmpImg = new BitmapImage();
                    bmpImg.BeginInit();
                    bmpImg.CacheOption = BitmapCacheOption.OnLoad;
                    bmpImg.StreamSource = stream;
                    bmpImg.EndInit();
                    bmpImg.Freeze();

                    return new Image()
                    {
                        Source = bmpImg,
                        Width = 180,
                        MaxHeight = 180,
                        Margin = ImgBoxMargin,
                        ToolTip = Path.GetFileName(path)
                    };
                }
            }
        }

        // From https://stackoverflow.com/a/1406853/9241163
        static bool IsFileReady(string path)
        {
            try
            {
                using (FileStream inputStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None))
                    return inputStream.Length > 0;
            }
            catch (Exception)
            {
                return false;
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
            MenuItem menuItem = sender as MenuItem;
            Image originalSource = (menuItem.Parent as ContextMenu).PlacementTarget as Image;
            // The ToolTip is the name of the image 
            string OriginalPath = Settings.ImgGalleryPath + "\\" + originalSource.ToolTip;

            using (FileStream Stream = new FileStream(OriginalPath, FileMode.Open, FileAccess.ReadWrite))
            {
                EditMetadataWindow metadataPopup = new EditMetadataWindow(OriginalPath, MetadataEditor.GetValuePairs(Stream));
                metadataPopup.Owner = Application.Current.MainWindow;
                metadataPopup.Show();
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
        }

        void OnImageClick(object o, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                // Freezing the image
                CamControl.Freeze();

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
            }
        }
    }

    // Only handels the FileSystemWatcher for the image gallery
    public partial class ImageGallery : StackPanel
    {
        FileSystemWatcher Watcher;

        // Initializes the FileSystemWatcher 
        void InitializeWatcher(string path)
        {
            Watcher = new FileSystemWatcher(path);

            // So if anything changes the FileSystemWatcher listens
            Watcher.Created += Watcher_Created;
            Watcher.Deleted += Watcher_Deleted;
            Watcher.Renamed += Watcher_Renamed;

            // Only fires events if a .png file changes
            Watcher.Filter = "*.png";
            Watcher.IncludeSubdirectories = false;
            Watcher.EnableRaisingEvents = true;
        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
            => UI.CurrentDispatcher.BeginInvoke(() => AddImg(e.FullPath), DispatcherPriority.Background);

        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
            => UI.CurrentDispatcher.BeginInvoke(() => RemoveImg(e.Name), DispatcherPriority.Background);

        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            Image renamedImg = Images[e.OldName];
            Images.Remove(e.OldName);
            Images.Add(e.Name, renamedImg);
            UI.CurrentDispatcher.BeginInvoke(() => renamedImg.ToolTip = e.Name, DispatcherPriority.Background);
        }
    }
}
