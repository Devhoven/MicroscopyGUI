using System;
using System.Windows;
using peak.core.nodes;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MicroscopeGUI.Helper.UIInteraction;
using MicroscopeGUI.MainWindowComponents.Controls;

namespace MicroscopeGUI
{
    // Slider controls with double values
    class FloatNodeControl : NodeControl
    {
        // Standard margins for the elements
        static Thickness LabelMargin = new Thickness(0, 5, 0, 5);
        static Thickness SliderMargin = new Thickness(5, 0, 5, 0);

        FloatNode Node;

        Slider Slider;
        // Contains the current input
        NumberInput NumInput;

        // Delays the frequency of accesses to the camera when the slider values changes
        Stopwatch DelayTimer = Stopwatch.StartNew();
        // A new value can only be set after a delay of MIN_ELAPSED_MS in ms
        const int MIN_ELAPSED_MS = 150;

        public override bool Enable
        {
            get => Slider.IsEnabled;
            set
            {
                // If it gets disabled, the foreground color is set to gray
                if (value)
                    Label.Foreground = Brushes.White;
                else
                    Label.Foreground = Brushes.Gray;
                Slider.IsEnabled = value;
            }
        }

        public FloatNodeControl(FloatNode node) : base(node.DisplayName())
        {
            Node = node;

            InitControls();

            Node.ChangedEvent += NodeChanged;
        }

        void InitControls()
        {
            Label.Margin = LabelMargin;

            // Slider and DecimalUpDown element with the right range and default value
            Slider = new Slider()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                AutoToolTipPlacement = AutoToolTipPlacement.TopLeft,
                AutoToolTipPrecision = 2,
                Margin = SliderMargin,
                Value = Node.Value(),
                Minimum = Node.Minimum(),
                Maximum = Node.Maximum(),
                Style = ResourceManager.GetResource<Style>("Horizontal_Slider")
            };
            
            NumInput = new NumberInput()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Value = Node.Value(),
                Minimum = Node.Minimum(),
                Maximum = Node.Maximum()
            };

            // Contains the slider and numinput
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
            RowDefinitions.Add(new RowDefinition());

            SetColumn(Slider, 0);
            SetRow(Slider, 1);
            Children.Add(Slider);

            SetRow(NumInput, 1);
            SetColumn(NumInput, 1);
            Children.Add(NumInput);

            // If the node has a constant increment, it is going to bet set to the slider
            if (Node.HasConstantIncrement())
            {
                Slider.IsSnapToTickEnabled = true;
                Slider.TickFrequency = Node.Increment();
            }

            Slider.ValueChanged += SliderValueChanged;

            // If the user is finished with his input, the final value is going to be set in this method
            Slider.PreviewMouseUp += (o, e) => SetValue();
            NumInput.ValueConfirmed += NumInputValueConfirmed;
        }

        // Updates the ranges from the controls, if they change
        private void NodeChanged(object sender, Node e)
        {
            NumInput.Minimum = Node.Minimum();
            NumInput.Maximum = Node.Maximum();

            Slider.Minimum = Node.Minimum();
            Slider.Maximum = Node.Maximum();
        }

        // Slider.Value is going to fire the ValueChanged event, then the SliderValueChanged method is going to be called, where the value will be updated
        private void NumInputValueConfirmed()
            => Slider.Value = NumInput.Value;

        private void SliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            NumInput.Value = Math.Round(e.NewValue, 2);
            ChangeValue();
        }

        // Gets called if the user sets a new value 
        void ChangeValue()
        {
            if (DelayTimer.ElapsedMilliseconds > MIN_ELAPSED_MS)
            {
                SetValue();
                DelayTimer.Restart();
            }
        }

        // Sets the value of the node
        void SetValue()
            => CamControl.SetNodeValue(() => Node.SetValue(Slider.Value));
    }
}
