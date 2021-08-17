using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
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
                    ImageQueue.CurrentFrameBitmap.Save(SaveDialog.FileName);

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
