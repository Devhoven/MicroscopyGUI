using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Text.RegularExpressions;

namespace MicroscopeGUI
{
    public partial class SettingsWindow : Window
    {
        static readonly Regex NumRegex = new Regex("[^0-9.]+"); // Regex that matches disallowed text

        public SettingsWindow()
        {
            InitializeComponent();

            PreviewKeyDown += (o, e) =>
            {
                if (e.Key == Key.Escape)
                    Close();
            };

            LineColorPicker.SelectedColor = Settings.LineColor.Color;
            LineThicknessTextBox.Text = Settings.LineThickness.ToString();
            LineTextColorPicker.SelectedColor = Settings.LineTextColor.Color;
            PathTextBlock.Text = Settings.ImgGalleryPath;
        }

        private void LineColorPickerColorChanged(object sender, RoutedEventArgs e) =>
            Settings.LineColor = new SolidColorBrush(LineColorPicker.SelectedColor);

        private void LineTextColorPickerColorChanged(object sender, RoutedEventArgs e) =>
            Settings.LineTextColor = new SolidColorBrush(LineTextColorPicker.SelectedColor);

        private void CheckIfNumeric(object sender, TextCompositionEventArgs e) =>
            e.Handled = NumRegex.IsMatch(e.Text);

        private void LineThicknessChanged(object sender, TextChangedEventArgs e)
        {
            if (((TextBox)sender).Text != string.Empty)
                Settings.LineThickness = int.Parse(((TextBox)sender).Text);
        }
    }
}
