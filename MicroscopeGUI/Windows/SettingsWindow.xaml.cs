using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System;

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
                {
                    Close();
                    e.Handled = true;
                }
            };

            LineColorPicker.SelectedColor = Settings.LineColor.Color;
            LineThicknessTextBox.Text = Settings.LineThickness.ToString();
            LineTextColorPicker.SelectedColor = Settings.LineTextColor.Color;
            PathTextBlock.Text = Settings.ImgGalleryPath;
        }

        private void LineColorPickerColorChanged(object sender, RoutedEventArgs e) 
            => Settings.LineColor = new SolidColorBrush(LineColorPicker.SelectedColor);

        private void LineTextColorPickerColorChanged(object sender, RoutedEventArgs e) 
            => Settings.LineTextColor = new SolidColorBrush(LineTextColorPicker.SelectedColor);

        private void CheckIfNumeric(object sender, TextCompositionEventArgs e) 
            => e.Handled = NumRegex.IsMatch(e.Text);

        private void LineThicknessChanged(object sender, TextChangedEventArgs e)
        {
            if (((TextBox)sender).Text != string.Empty)
                Settings.LineThickness = int.Parse(((TextBox)sender).Text);
        }


        // From https://stackoverflow.com/a/19780697/9241163
        // Witht this bit of code the window always start at the cursor position
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            MoveBottomRightEdgeOfWindowToMousePosition();
        }

        private void MoveBottomRightEdgeOfWindowToMousePosition()
        {
            var transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
            var mouse = transform.Transform(GetMousePosition());
            Left = mouse.X - ActualWidth;
            Top = mouse.Y - ActualHeight;

            if (Left < 0)
                Left = 0;
            if (Top < 0)  
                Top = 0;

            Point GetMousePosition()
            {
                System.Drawing.Point point = System.Windows.Forms.Control.MousePosition;
                return new Point(point.X, point.Y);
            }
        }
    }
}
