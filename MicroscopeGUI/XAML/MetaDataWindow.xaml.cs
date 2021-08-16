using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using Application = System.Windows.Application;
using Brushes = System.Windows.Media.Brushes;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using TextBox = System.Windows.Controls.TextBox;

namespace MicroscopeGUI
{
    public partial class MetaDataWindow : Window
    {
        List<DataEntry> Entries = new List<DataEntry>();

        string OriginalPath = null;

        public MetaDataWindow()
        {
            InitializeComponent();

            AddDataEntries(Control.GetXMLString());

            AddDataEntry("Comment", "", true).ValueTextBox.Focus();

            KeyDown += MetaDataWindowKeyDown;

            Closed += WindowClosing;
        }

        public MetaDataWindow(string OriginalPath, Dictionary<string, string> KeyValuePairs)
        {
            InitializeComponent();

            this.OriginalPath = OriginalPath;

            AddDataEntries(KeyValuePairs);

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
                string TempPath = OriginalPath.Substring(0, OriginalPath.IndexOf(".png")) + "-temp.png";
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

        class DataEntry : DockPanel 
        {
            static int Count = 0;

            public string Key
            {
                get => KeyTextBox.Text;
            }

            public string Value
            {
                get => ValueTextBox.Text;
            }

            public TextBox KeyTextBox;
            public TextBox ValueTextBox;
            new StackPanel Parent;

            public DataEntry(StackPanel Parent, string Key, string Value, bool KeyEditable = false, bool ValueEditable = false)
            {
                this.Parent = Parent;

                KeyTextBox = new TextBox()
                {
                    Text = Key,
                    IsReadOnly = !KeyEditable,
                    CaretBrush = Brushes.White,
                    BorderBrush = Brushes.White,
                    Foreground = Count % 2 == 0 ? Brushes.White : Brushes.Black,
                    Background = Count % 2 == 0 ? Brushes.Transparent : Brushes.LightBlue,
                    BorderThickness = new Thickness(2),
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Width = 250,
                    Height = 40
                };
                ValueTextBox = new TextBox()
                {
                    Text = Value,
                    IsReadOnly = !ValueEditable,
                    CaretBrush = Brushes.White,
                    BorderBrush = Brushes.White,
                    Foreground = Count % 2 == 0 ? Brushes.White : Brushes.Black,
                    Background = Count % 2 == 0 ? Brushes.Transparent : Brushes.LightBlue,
                    BorderThickness = new Thickness(2),
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Height = 40
                };

                KeyTextBox.PreviewKeyDown += KeyTextBoxKeyDown;

                Children.Add(KeyTextBox);
                SetDock(KeyTextBox, Dock.Left);
                Children.Add(ValueTextBox);
                SetDock(ValueTextBox, Dock.Right);

                Count++;
            }

            private void KeyTextBoxKeyDown(object sender, KeyEventArgs e)
            {
                if (e.Key == System.Windows.Input.Key.Back)
                {
                    if ((sender as TextBox).Text == string.Empty)
                    {
                        Parent.Children.Remove(this);
                    }
                }
            }
        }
    }
}
