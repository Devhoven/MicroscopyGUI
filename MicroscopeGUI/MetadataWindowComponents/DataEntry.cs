using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using TextBox = System.Windows.Controls.TextBox;

namespace MicroscopeGUI.MetadataWindowComponents
{
    public class DataEntry : DockPanel
    {
        static int Count = 1;
        static Brush FirstBackgroundColor = (Brush)new BrushConverter().ConvertFrom("#FF2f2f30");
        static Brush FirstForegroundColor = Brushes.White;

        static Brush SecondBackgroundColor = (Brush)new BrushConverter().ConvertFrom("#FF434346");
        static Brush SecondForegroundColor = Brushes.White;

        static Thickness Padding = new Thickness(5);
        static Thickness BorderThickness = new Thickness(0);

        static int KeyWidth = 250;
        static int RowHeight = 40;

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
                Foreground = Count % 2 == 0 ? FirstForegroundColor : SecondForegroundColor,
                Background = Count % 2 == 0 ? FirstBackgroundColor : SecondBackgroundColor,
                BorderThickness = BorderThickness,
                Padding = Padding,
                VerticalContentAlignment = VerticalAlignment.Center,
                Width = KeyWidth,
                Height = RowHeight,
                // iTXt chunks have a limit for the key, which is at 50 chars
                MaxLength = 50
            };
            ValueTextBox = new TextBox()
            {
                Text = Value,
                IsReadOnly = !ValueEditable,
                Foreground = Count % 2 == 0 ? FirstForegroundColor : SecondForegroundColor,
                Background = Count % 2 == 0 ? FirstBackgroundColor : SecondBackgroundColor,
                BorderThickness = BorderThickness,
                Padding = Padding,
                VerticalContentAlignment = VerticalAlignment.Center,
                Height = RowHeight
            };

            KeyTextBox.PreviewKeyDown += KeyTextBoxKeyDown;
            ValueTextBox.PreviewKeyDown += ValueTextBoxKeyDown;

            Children.Add(KeyTextBox);
            SetDock(KeyTextBox, Dock.Left);
            Children.Add(ValueTextBox);
            SetDock(ValueTextBox, Dock.Right);

            Count++;
        }

        private void ValueTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Back)
            {
                if ((sender as TextBox).Text == string.Empty)
                {
                    KeyTextBox.Focus();
                }
            }
        }

        private void KeyTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Back)
            {
                if ((sender as TextBox).Text == string.Empty)
                {
                    int Index = Parent.Children.IndexOf(this);
                    // Removing the current element
                    Parent.Children.RemoveAt(Index);
                    // If this is not the last element, focus the value text box from the entry before
                    if (Index > 0)
                    {
                        (Parent.Children[Index - 1] as DataEntry).ValueTextBox.Focus();
                    }
                }
            }
        }
    }
}
