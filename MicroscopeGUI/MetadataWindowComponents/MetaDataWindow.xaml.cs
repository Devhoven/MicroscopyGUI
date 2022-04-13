using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace MicroscopeGUI.MetadataWindowComponents
{
    public partial class MetadataWindow : Window
    {
        protected List<DataEntry> Entries = new List<DataEntry>();

        private string SavePath;

        public MetadataWindow(bool addComment = true) 
        {
            Initialize();

            if (addComment)
                AddDataEntry("Comment", "", true).ValueTextBox.Focus();
        }

        protected void Initialize()
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
                DataEntry newEntry = new DataEntry(MetadataCon, string.Empty, string.Empty, true, true);
                MetadataCon.Children.Add(newEntry);
                newEntry.KeyTextBox.Focus();
                Entries.Add(newEntry);
            }
            else if (e.Key == Key.Escape)
                Close();
        }

        protected DataEntry AddDataEntry(string key, string value, bool valueEditable = false)
        {
            DataEntry newEntry = new DataEntry(MetadataCon, key, value, false, valueEditable);
            MetadataCon.Children.Add(newEntry);
            Entries.Add(newEntry);
            return newEntry;
        }

        protected virtual void ConfirmClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Title = "Save file",
                Filter = "Png|*.png"
            };

            if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SavePath = saveDialog.FileName;

                CamControl.SavedFrame += () =>
                {
                    using (FileStream Stream = new FileStream(SavePath, FileMode.Open, FileAccess.ReadWrite))
                    {
                        foreach (DataEntry Entry in Entries)
                            MetadataEditor.AddiTXt(Stream, Entry.Key, Entry.Value);
                    }

                    // Updates the image gallery
                    string FolderPath = Path.GetDirectoryName(SavePath);
                    UI.CurrentGallery.UpdatePath(FolderPath);
                };

                CamControl.SafeFrameTo(SavePath);

                Close();
            }
        }

        private void CancelClick(object sender, RoutedEventArgs e) =>
            Close();
    }
}
