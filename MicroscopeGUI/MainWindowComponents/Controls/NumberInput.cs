using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MicroscopeGUI.MainWindowComponents.Controls
{
    internal class NumberInput : TextBox
    {
        double _Value;

        public double Value
        {
            get => _Value;
            set
            {
                _Value = Math.Clamp(value, Minimum, Maximum);
                Text = _Value.ToString("N2");
            }
        }
        public double Minimum = double.MinValue;
        public double Maximum = double.MaxValue;

        public event Action ValueConfirmed;

        public NumberInput()
        {
            // It's somehow white on white by default???
            Foreground = Brushes.Black;
            DataObject.AddPastingHandler(this, TextBoxPasting);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                double.TryParse(Text, out double val);
                Value = val;
                ValueConfirmed.Invoke();
            }
        }

        private static readonly Regex _regex = new Regex("[0-9.,-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
            => _regex.IsMatch(text);

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
            => e.Handled = !IsTextAllowed(e.Text);

        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!IsTextAllowed(text))
                    e.CancelCommand();
            }
            else
                e.CancelCommand();
        }
    }
}
