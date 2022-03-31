using MicroscopeGUI.MainWindowComponents.Controls;
using peak.core.nodes;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace MicroscopeGUI
{
    // Slider controls with double values
    class FloatSliderControl : Control
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

        public FloatSliderControl(FloatNode node, ControlCon Parent, bool EnabledByDefault = true) : base(node.DisplayName(), Parent)
        {
            Node = node;

            Label.Margin = LabelMargin;
            Parent.Children.Add(Label);

            InitControls();

            Enable = EnabledByDefault;

            // If the node is cacheable the controls don't need to be updated
            if (!Node.IsCacheable())
                Node.ChangedEvent += NodeChanged;
        }

        void InitControls()
        {
            // Contains the slider and numinput
            Grid panel = new Grid();
            panel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            panel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });

            // Slider and DecimalUpDown element with the right range and default value
            Slider = new Slider()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                AutoToolTipPlacement = System.Windows.Controls.Primitives.AutoToolTipPlacement.TopLeft,
                AutoToolTipPrecision = 2,
                Margin = SliderMargin,
                Value = Node.Value(),
                Minimum = Node.Minimum(),
                Maximum = Node.Maximum(),
                Style = (Style)Application.Current.FindResource("Horizontal_Slider")
            };
            NumInput = new NumberInput()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Value = Node.Value(),
                Minimum = Node.Minimum(),
                Maximum = Node.Maximum()
            };

            Grid.SetColumn(Slider, 0);
            Grid.SetColumn(NumInput, 1);
            panel.Children.Add(Slider);
            panel.Children.Add(NumInput);

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

            Parent.Children.Add(panel);
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

        // Thread safe value setting
        public override void SetValue(object Value) =>
            UI.CurrentDispatcher.Invoke(() => Slider.Value = Convert.ToDouble(Value));

        // Sets the value of the node
        void SetValue()
            => Parent.SetNode(() => Node.SetValue(Slider.Value));

        public override object GetValue() =>
            Slider.Value;

        protected override string GetName() =>
            Node.DisplayName();
    }
}
