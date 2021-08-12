using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MicroscopeGUI
{
    /// <summary>
    /// Interaktionslogik für MetaDataWindow.xaml
    /// </summary>
    public partial class MetaDataWindow : Window
    {
        string filename;
        public MetaDataWindow(string filename)
        {
            InitializeComponent();

            this.filename = filename;
            CommentBox.Focus();
            LoadText();
        }

        public void LoadText()
        {
            TitleBox.Text = filename.Substring(filename.LastIndexOf("\\") + 1);
        }

        private void SubmitClick(object sender, RoutedEventArgs e)
        {
            //ImageQueue.CurrentFrameBitmap.Save(filename);
            using (FileStream fs = new FileStream("C:\\Users\\Lukas\\Pictures\\unknown.png", FileMode.Open, FileAccess.Write, FileShare.Write)) //hier gibt es probleme mit exceptions in der metadataeditor klasse
            {
                MetadataEditor.AddiTXt(fs, "Comment", CommentBox.Text);
            }
            UserInfo.SetInfo("Saved the current frame to " + filename);
            Close();
        }

        private void AbortClick(object sender, RoutedEventArgs e) =>
            Close();
    }
}
