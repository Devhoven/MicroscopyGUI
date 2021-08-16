using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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
    public partial class SettingsWindow : Window
    {
        static readonly Regex NumRegex = new Regex("[^0-9.]+"); // Regex that matches disallowed text

        public SettingsWindow()
        {
            InitializeComponent();

            LineColorPicker.SelectedColor = Settings.LineColor.Color;
            LineThicknessTextBox.Text = Settings.LineThickness.ToString();
            LineTextColorPicker.SelectedColor = Settings.LineTextColor.Color;
            PathTextBlock.Text = Settings.ImgGalleryPath;
        }

        private void LineColorPickerColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e) =>
            Settings.LineColor = new SolidColorBrush((Color)e.NewValue);

        private void LineTextColorPickerColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e) =>
            Settings.LineTextColor = new SolidColorBrush((Color)e.NewValue);

        private void CheckIfNumeric(object sender, TextCompositionEventArgs e) =>
            e.Handled = NumRegex.IsMatch(e.Text);

        private void LineThicknessChanged(object sender, TextChangedEventArgs e)
        {
            if (((TextBox)sender).Text != string.Empty)
                Settings.LineThickness = int.Parse(((TextBox)sender).Text);
        }
    }
}
