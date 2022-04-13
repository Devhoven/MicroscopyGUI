using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MicroscopeGUI.MetadataWindowComponents
{
    class EditMetadataWindow : MetadataWindow
    {
        string OriginalPath = null;

        public EditMetadataWindow(string OriginalPath, Dictionary<string, string> KeyValuePairs) : base(false)
        {
            ConfirmBtn.Content = "Edit Image";

            this.OriginalPath = OriginalPath;

            AddDataEntries(KeyValuePairs);
        }

        void AddDataEntries(Dictionary<string, string> KeyValuePairs)
        {
            foreach (KeyValuePair<string, string> Pair in KeyValuePairs)
                AddDataEntry(Pair.Key, Pair.Value, true);
        }

        protected override void ConfirmClick(object sender, RoutedEventArgs e)
        {
            // Small cheat for removing the metadata, since I can't be bothered anymore
            // It is creating a new bitmap from the original image and saves it again
            // This new image does not have any medatada
            // So we delete the old file and rename the temporary file to be the old / new one

            string tmpPath = Path.GetTempFileName();
            using (Bitmap bmp = new Bitmap(OriginalPath))
                bmp.Save(tmpPath);
            File.Delete(OriginalPath);
            File.Move(tmpPath, OriginalPath);

            using (FileStream stream = new FileStream(OriginalPath, FileMode.Open, FileAccess.ReadWrite))
            {
                foreach (DataEntry entry in Entries)
                    MetadataEditor.AddiTXt(stream, entry.Key, entry.Value);
            }
            Close();
        }
    }
}
