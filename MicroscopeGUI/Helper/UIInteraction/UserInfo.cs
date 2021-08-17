using System.Windows.Controls;
using System.Timers;
using System.Windows.Media;
using System.Windows;

namespace MicroscopeGUI
{
    static class UserInfo
    {
        static Label InfoLabel;
        // Just used for aesthetics
        static Border InfoLabelBorder;
        static Timer TextTimer = new Timer(6000);

        static UserInfo()
        {
            // Retreiving the reference to the InfoLabel and its border from the MainWindow
            InfoLabel = (Application.Current.MainWindow as UI).InfoLabel;
            InfoLabelBorder = (Application.Current.MainWindow as UI).InfoLabelBorder;
        }

        // Creates a timer, which will delete the text after a few seconds
        public static void SetInfo(string Text)
        {
            // Setting the default visual attributes of the InfoLabel
            InfoLabel.Foreground = Brushes.White;
            InfoLabel.FontWeight = FontWeights.Normal;
            InfoLabelBorder.Background = Brushes.Transparent;
            InfoLabel.Content = Text;
            // Stopping the timer, if it was running from a previous UserInfo-call
            TextTimer.Stop();

            // If the timer elapses, the text from the label and the background color from the border are going to be reset
            TextTimer.Elapsed += (s, e) =>
            {
                UI.CurrentDispatcher.Invoke(() =>
                {
                    InfoLabel.Content = string.Empty;
                    InfoLabelBorder.Background = Brushes.Transparent;
                });
                TextTimer.Stop();
            };
            TextTimer.Start();
        }

        // Function which allows for custom colors
        public static void SetInfo(string Text, Brush BackgroundColor, Brush ForegroundColor)
        {
            SetInfo(Text);
            InfoLabel.Foreground = ForegroundColor;
            InfoLabelBorder.Background = BackgroundColor;
        }

        // Function which shows the label with bold text and a red background color
        public static void SetErrorInfo(string Text)
        {
            SetInfo(Text);
            InfoLabel.Foreground = Brushes.White;
            InfoLabel.FontWeight = FontWeights.DemiBold;
            InfoLabelBorder.Background = Brushes.Red;
        }
    }
}
