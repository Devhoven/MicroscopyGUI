using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
            {
                SaveFileDialog SaveDialog = new SaveFileDialog();
                SaveDialog.Title = "Save file";
                SaveDialog.Filter = "Png|*.png";

                if (SaveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    // Quick n dirty fix, in order to apply the post processing to the saved frame
                    Bitmap savingBitmap = (Bitmap)ImageQueue.CurrentFrameBitmap.Clone();
                    float brightness =  UI.FrameEffects.Brightness;
                    float contrast   =  UI.FrameEffects.Contrast;
                    float amountR    =  UI.FrameEffects.AmountR;
                    float amountG    =  UI.FrameEffects.AmountG;
                    float amountB    =  UI.FrameEffects.AmountB;
                    Thread t = new Thread(new ThreadStart(SaveImg));
                    UserInfo.SetInfo("Saving image...");
                    t.Start();

                    unsafe void SaveImg()
                    {
                        float r, g, b;
                        BitmapData bmpData = null;
                        bmpData = savingBitmap.LockBits(
                            new Rectangle(0, 0, savingBitmap.Width, savingBitmap.Height),
                            ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                        for (int y = 0; y < savingBitmap.Height; y++)
                        {
                            byte* srcRow = (byte*)bmpData.Scan0 + (y * bmpData.Stride);
                            for (int x = 0; x < savingBitmap.Width; x++)
                            {
                                r = ((srcRow[x * 4 + 2] / 255f * amountR - 0.5f)) * MathF.Max(contrast, 0) + 0.5f + brightness;
                                g = ((srcRow[x * 4 + 1] / 255f * amountG - 0.5f)) * MathF.Max(contrast, 0) + 0.5f + brightness;
                                b = ((srcRow[x * 4 + 0] / 255f * amountB - 0.5f)) * MathF.Max(contrast, 0) + 0.5f + brightness;

                                srcRow[x * 4 + 2] = (byte)MathF.Max(MathF.Min(r * 255, 255), 0);
                                srcRow[x * 4 + 1] = (byte)MathF.Max(MathF.Min(g * 255, 255), 0);
                                srcRow[x * 4 + 0] = (byte)MathF.Max(MathF.Min(b * 255, 255), 0);
                            }
                        }

                        savingBitmap.UnlockBits(bmpData);

                        savingBitmap.Save(SaveDialog.FileName);

                        UI.CurrentDispatcher.Invoke(new Action(() =>
                        {
                            using (FileStream Stream = new FileStream(SaveDialog.FileName, FileMode.Open, FileAccess.ReadWrite))
                            {
                                foreach (DataEntry Entry in Entries)
                                {
                                    MetadataEditor.AddiTXt(Stream, Entry.Key, Entry.Value);
                                }
                            }

                            // Updates the image gallery
                            string FolderPath = SaveDialog.FileName.Substring(0, SaveDialog.FileName.LastIndexOf("\\"));
                            (Application.Current.MainWindow as UI).ImgGallery.UpdatePath(FolderPath);
                            UserInfo.SetInfo("Saved the image and updated the path");
                        }
                        ));
                    }

                    Close();
                }
            }
            else
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
        }

        private void CancelClick(object sender, RoutedEventArgs e) =>
            Close();
    }
}
