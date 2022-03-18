using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml;
using Application = System.Windows.Application;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace MicroscopeGUI
{
    public partial class MetaDataWindow : Window
    {
        List<DataEntry> Entries = new List<DataEntry>();

        string OriginalPath = null;

        public MetaDataWindow() 
        {
            Initialize();

            AddDataEntries(Control.GetXMLString());

            AddDataEntry("Comment", "", true).ValueTextBox.Focus();
        }

        public MetaDataWindow(string OriginalPath, Dictionary<string, string> KeyValuePairs)
        {
            Initialize();

            this.OriginalPath = OriginalPath;

            AddDataEntries(KeyValuePairs);
        }

        void Initialize()
        {
            InitializeComponent();

            KeyDown += MetaDataWindowKeyDown;
            Closed += WindowClosing;
        }

        // So the main window does not get unfocused
        private void WindowClosing(object sender, EventArgs e) =>
            Owner.Activate();

        private void MetaDataWindowKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DataEntry NewEntry = new DataEntry(MetadataCon, string.Empty, string.Empty, true, true);
                MetadataCon.Children.Add(NewEntry);
                NewEntry.KeyTextBox.Focus();
                Entries.Add(NewEntry);
            }
            else if (e.Key == Key.Escape)
                Close();
        }

        void AddDataEntries(string XML)
        {
            // Removing all of the whitespace
            Regex.Replace(XML, @"\s+", "");
            XmlDocument Document = new XmlDocument();
            Document.LoadXml(XML);
            XmlElement Root = Document.DocumentElement;
            XmlNodeList Nodes = Root.ChildNodes;

            // Going through all of the nodes inside the root node except the ones for post processing
            foreach (XmlNode Node in Nodes)
            {
                if (Node.Name == "Contrast")
                    break;
                
                AddDataEntry(Node.Name, Node.InnerText);
            }
        }

        void AddDataEntries(Dictionary<string, string> KeyValuePairs)
        {
            foreach(KeyValuePair<string, string> Pair in KeyValuePairs)
                AddDataEntry(Pair.Key, Pair.Value, true);
        }

        DataEntry AddDataEntry(string Key, string Value, bool ValueEditable = false)
        {
            DataEntry NewEntry = new DataEntry(MetadataCon, Key, Value, false, ValueEditable);
            MetadataCon.Children.Add(NewEntry);
            Entries.Add(NewEntry);
            return NewEntry;
        }

        private void SaveImgClick(object sender, RoutedEventArgs e)
        {
            if (OriginalPath is null)
                SaveImgDialog();
            else
                RemoveMetadata();
        }

        void SaveImgDialog()
        {
            SaveFileDialog SaveDialog = new SaveFileDialog();
            SaveDialog.Title = "Save file";
            SaveDialog.Filter = "Png|*.png";

            if (SaveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SaveImg(SaveDialog.FileName);

                Close();
            }

        }

        // Counts how often the method failed, so that we won't end up in an infinite loop
        static int failCount = 0;
        // Saves the current frame at the given file location
        void SaveImg(string fileName)
        {
            Bitmap savingBitmap;
            try
            {
                // This can sometime fail because of memory issues
                // That's the reason for the try - catch
                savingBitmap = ImageQueue.CurrentFrameBitmap.KernellDllCopyBitmap();
            }
            catch
            {
                // After it failed 5 times, it will inform the user and close the method
                failCount++;
                if (failCount > 5)
                {
                    Dispatcher.Invoke(() => UserInfo.SetErrorInfo("The image could not be saved, please try again"));
                    failCount = 0;
                    return;
                }

                // Trying to copy the bitmap again
                SaveImg(fileName);
                return;
            }

            // Retreiving all the post processing values from the UI 
            // brightness is a value between -1 and 1, and we need it to be between -255 and 255
            float brightness = UI.FrameEffects.Brightness * 255;
            float contrast = UI.FrameEffects.Contrast;
            float amountR = UI.FrameEffects.AmountR;
            float amountG = UI.FrameEffects.AmountG;
            float amountB = UI.FrameEffects.AmountB;

            // Starting it in a different thread, so the UI won't freeze if it takes a bit longer ;)
            Thread t = new Thread(new ThreadStart(ProcessImg));
            UserInfo.SetInfo("Saving image...");
            t.Start();

            // Processes the image, saves it to the given location and adds all the metadata
            unsafe void ProcessImg()
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                // Since we need to manipulate the pixels of this bitmap really fast, we have to lock the bits for faster access time
                BitmapData bmpData = savingBitmap.LockBits(
                    new Rectangle(0, 0, savingBitmap.Width, savingBitmap.Height),
                    ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                // Saving them in seperate variables, so we don't need to access the bmpData - object
                int width = bmpData.Width;
                int height = bmpData.Height;   
                int stride = bmpData.Stride;
                IntPtr scan0 = bmpData.Scan0;

                int threadCount = 10;
                // How many rows (or less) are processed by each thread
                int maxRowCount = height / threadCount;

                // Is going to hold all of the generated threads
                Thread[] threads = new Thread[(int)Math.Ceiling(height / (float)maxRowCount)];

                // Creating all of the threads, except the last one
                for (int i = 0; i < threads.Length - 1; i++)
                {
                    threads[i] = new Thread(new ParameterizedThreadStart(ProcessRows));
                    threads[i].Start((i * maxRowCount, maxRowCount));
                }
                // Creating the last thread, seperately so we can give it all of the leftover rows
                threads[^1] = new Thread(new ParameterizedThreadStart(ProcessRows));
                threads[^1].Start(((threads.Length - 1) * maxRowCount, height - (threads.Length - 1) * maxRowCount));

                // It has to accept a object for a ParameterizedThreadStart
                // Can't use a lambda expression, since i is changing "too fast"
                unsafe void ProcessRows(object param)
                {
                    (int yStart, int count) = ((int, int))param;

                    int x3;
                    // Goes through all of the given rows and calculates the post processing effects, just like in the EffectShader.hlsl
                    for (int y = yStart; y < yStart + count; y++)
                    {
                        byte* srcRow = (byte*)bmpData.Scan0 + (y * bmpData.Stride);
                        for (int x = 0; x < width; x++)
                        {
                            x3 = x * 3;
                            srcRow[x3 + 2] = (byte)Math.Clamp((srcRow[x3 + 2] * amountR - 128) * contrast + 128 + brightness, 0, 255);
                            srcRow[x3 + 1] = (byte)Math.Clamp((srcRow[x3 + 1] * amountG - 128) * contrast + 128 + brightness, 0, 255);
                            srcRow[x3 + 0] = (byte)Math.Clamp((srcRow[x3 + 0] * amountB - 128) * contrast + 128 + brightness, 0, 255);
                        }
                    }
                }

                // Waiting for the threads to end
                for (int i = 0; i < threads.Length; i++)
                    threads[i].Join();

                // Saving the bitmap and cleaning up
                savingBitmap.UnlockBits(bmpData);
                savingBitmap.Save(fileName);
                savingBitmap.Dispose();

                // Updates the image gallery and informs the user
                UI.CurrentDispatcher.Invoke(new Action(() =>
                {
                    // Updates the metadata of the file
                    using (FileStream Stream = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite))
                        foreach (DataEntry Entry in Entries)
                            MetadataEditor.AddiTXt(Stream, Entry.Key, Entry.Value);

                    // Updates the image gallery
                    string FolderPath = fileName.Substring(0, fileName.LastIndexOf("\\"));
                    (Application.Current.MainWindow as UI).ImgGallery.UpdatePath(FolderPath);
                    stopwatch.Stop();
                    UserInfo.SetInfo("Saved the image and updated the path (" + stopwatch.ElapsedMilliseconds + "ms)");
                }));
            }

            failCount = 0;
        }

        void RemoveMetadata()
        {
            // Small cheat for removing the metadata, since I can't be bothered anymore
            // It is creating a new bitmap from the original image and saves it again
            // This new image does not have any medatada
            // So we delete the old file and rename the temporary file to be the old / new one
            string TempPath = OriginalPath.Substring(0, OriginalPath.ToLower().IndexOf(".png")) + "-temp.png";
            using (Bitmap Bmp = new Bitmap(OriginalPath))
                Bmp.Save(TempPath);
            File.Delete(OriginalPath);
            File.Move(TempPath, OriginalPath);

            using (FileStream Stream = new FileStream(OriginalPath, FileMode.Open, FileAccess.ReadWrite))
            {
                foreach (DataEntry Entry in Entries)
                {
                    MetadataEditor.AddiTXt(Stream, Entry.Key, Entry.Value);
                }
            }
            Close();
        }

        private void CancelClick(object sender, RoutedEventArgs e) =>
            Close();
    }
}
