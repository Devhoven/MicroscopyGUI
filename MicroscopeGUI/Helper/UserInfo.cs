using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Timers;
using System.Windows.Input;

namespace MicroscopeGUI
{
    static class UserInfo
    {
        public static Label InfoLabel
        {
            set
            {
                _InfoLabel = value;
            }
        }

        static Label _InfoLabel;
        static Timer TextTimer = new Timer(4000);

        // Creates a timer, which will delete the text after a few seconds
        public static void SetInfo(string Text)
        {
            _InfoLabel.Content = Text;
            _InfoLabel.Visibility = System.Windows.Visibility.Visible;
            TextTimer.Stop();

            TextTimer.Elapsed += (s, e) =>
            {
                UI.CurrentDispatcher.Invoke(() => _InfoLabel.Content = string.Empty);
                UI.CurrentDispatcher.Invoke(() => _InfoLabel.Visibility = System.Windows.Visibility.Collapsed);
                TextTimer.Stop();
            };
            TextTimer.Start();
        }
    }
}
