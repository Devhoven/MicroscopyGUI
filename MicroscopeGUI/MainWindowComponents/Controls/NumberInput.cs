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
                // If the value exceeds 1000, a k is going to be appended for readabillity
                if (value > 1000)
                    Text = (_Value / 1000).ToString("N1") + "k";
                else
                    Text = _Value.ToString("N2");
                // Replacing the commas with dots, since they are more conventional 
                Text = Text.Replace(',', '.');
            }
        }
        public double Minimum = double.MinValue;
        public double Maximum = double.MaxValue;

        public event Action ValueConfirmed;

        public NumberInput()
        {
            Foreground = Brushes.Black;
            DataObject.AddPastingHandler(this, TextBoxPasting);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                bool containsK = Text.EndsWith("k");
                // Replacing all dots with commas, and the k if it exists, otherwise it can't be parsed
                Text = Text.Replace('.', ',').Replace("k", string.Empty);
                double.TryParse(Text, out double val);
                // If the field contained a k, the final value has to be multiplied by 1000
                if (containsK)
                    val *= 1000;
                Value = val;
                ValueConfirmed.Invoke();
            }
        }

        private static readonly Regex NUM_REGEX = new Regex("[0-9k.,-]"); //regex that matches allowed text
        
        private static bool IsTextAllowed(string text)
            => NUM_REGEX.IsMatch(text);

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
            => e.Handled = !IsTextAllowed(e.Text) || (Text.EndsWith("k") && e.Text == "k");

        // Prevents the user from pasting text into the number input
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
