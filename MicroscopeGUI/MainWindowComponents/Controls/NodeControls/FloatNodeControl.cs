using System;
using System.Windows;
using peak.core.nodes;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MicroscopeGUI.Helper.UIInteraction;
using MicroscopeGUI.MainWindowComponents.Controls;
using MicroscopeGUI.IDSPeak;
using System.Threading.Tasks;

namespace MicroscopeGUI.MainWindowComponents.Controls.NodeControls
{
    // Slider controls with double values
    class FloatNodeControl : ControlBase
    {
        FloatNode Node;

        RangeInput ValInput;

        // Delays the frequency of accesses to the camera when the slider values changes
        Stopwatch DelayTimer = Stopwatch.StartNew();
        // A new value can only be set after a delay of MIN_ELAPSED_MS in ms
        const int MIN_ELAPSED_MS = 50;

        public override bool Enable
        {
            get => ValInput.IsEnabled;
            set
            {
                // If it gets disabled, the foreground color is set to gray
                if (value)
                    Label.Foreground = Brushes.White;
                else
                    Label.Foreground = Brushes.Gray;
                ValInput.IsEnabled = value;
            }
        }

        public FloatNodeControl(FloatNode node, ControlCon.ControlNodeInfo element) : base(element.DisplayName)
        {
            Node = node;

            InitControls();

            Node.ChangedEvent += NodeChanged;
        }

        private void InitControls()
        {
            ValInput = new RangeInput(Node.Value(), Node.Minimum(), Node.Maximum(), Node.HasConstantIncrement() ? Node.Increment() : 0);

            RowDefinitions.Add(new RowDefinition());

            SetColumn(ValInput, 0);
            SetRow(ValInput, 1);
            Children.Add(ValInput);

            ValInput.OnValueChanged += ValInputOnValueChanged;
            ValInput.OnValueConfirmed += ValInputOnValueConfirmed;
        }

        private void ValInputOnValueConfirmed(double newVal)
            => SetValue(newVal);

        private void ValInputOnValueChanged(double newVal)
            => ChangeValue(newVal);

        // Updates the ranges from the controls, if they change
        private void NodeChanged(object sender, Node e)
            => ValInput.ChangeRange(Node.Minimum(), Node.Maximum());

        // Gets called if the user sets a new value 
        private void ChangeValue(double newVal)
        {
            if (DelayTimer.ElapsedMilliseconds > MIN_ELAPSED_MS)
            {
                SetValue(newVal);
                DelayTimer.Restart();
            }
        }

        // Sets the value of the node
        private void SetValue(double newVal)
            => CamControl.SetNodeValue(() => Node.SetValue(newVal));
    }
}
