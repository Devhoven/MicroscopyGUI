using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Timers;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;

namespace MicroscopeGUI
{
    static class UserInfo
    {
        static Label InfoLabel;
        static Border InfoLabelBorder;
        static Timer TextTimer = new Timer(6000);

        static UserInfo()
        {
            InfoLabel = (Application.Current.MainWindow as UI).InfoLabel;
            InfoLabelBorder = (Application.Current.MainWindow as UI).InfoLabelBorder;
        }

        // Creates a timer, which will delete the text after a few seconds
        public static void SetInfo(string Text)
        {
            InfoLabel.Foreground = Brushes.White;
            InfoLabel.FontWeight = FontWeights.Normal;
            InfoLabelBorder.Background = Brushes.Transparent;
            InfoLabel.Content = Text;
            TextTimer.Stop();

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

        public static void SetInfo(string Text, Brush BackgroundColor, Brush ForegroundColor)
        {
            SetInfo(Text);
            InfoLabel.Foreground = ForegroundColor;
            InfoLabelBorder.Background = BackgroundColor;
        }

        public static void SetUrgentInfo(string Text)
        {
            SetInfo(Text);
            InfoLabel.Foreground = Brushes.White;
            InfoLabel.FontWeight = FontWeights.DemiBold;
            InfoLabelBorder.Background = Brushes.Red;
        }
    }
}
