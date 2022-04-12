using MicroscopeGUI.Helper.UIInteraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MicroscopeGUI.MainWindowComponents.Controls
{
    // A combination of a Slider and a NumberInput
    class RangeInput : Grid
    {
        // Standard margin for the slider
        static Thickness SliderMargin = new Thickness(5, 0, 5, 0);

        public delegate void ValueChanged(double newVal);
        public delegate void ValueConfirmed(double newVal);

        // Fires when the slider value changes
        public event ValueChanged OnValueChanged;
        // Fires when enter was pressed in the NumberInput or the MouseUp event from the slider got called
        public event ValueConfirmed OnValueConfirmed;

        public double Value => NumInput.Value;

        Slider Slider;
        // Contains the current input
        NumberInput NumInput;

        public RangeInput(double startVal, double minVal, double maxVal, double increment = 0)
        {
            InitControls(startVal, minVal, maxVal);
            AddToGrid();

            if (increment != 0)
            {
                Slider.IsSnapToTickEnabled = true;
                Slider.TickFrequency = increment;
            }

            BindEvents();
        }

        void InitControls(double startVal, double minVal, double maxVal)
        {
            // Slider and DecimalUpDown element with the right range and default value
            Slider = new Slider()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                AutoToolTipPlacement = AutoToolTipPlacement.TopLeft,
                AutoToolTipPrecision = 2,
                Margin = SliderMargin,
                Value = startVal,
                Minimum = minVal,
                Maximum = maxVal,
                Style = ResourceManager.GetResource<Style>("Horizontal_Slider")
            };

            NumInput = new NumberInput()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Value = startVal,
                Minimum = minVal,
                Maximum = maxVal
            };
        }

        void AddToGrid()
        {
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });

            SetColumn(Slider, 0);
            SetRow(Slider, 1);
            Children.Add(Slider);

            SetRow(NumInput, 1);
            SetColumn(NumInput, 1);
            Children.Add(NumInput);
        }
    
        void BindEvents()
        {
            // Invoking the events of the class
            Slider.ValueChanged += (o, e) =>
                OnValueChanged.Invoke(e.NewValue);
            Slider.PreviewMouseUp += (o, e) =>
                OnValueConfirmed.Invoke(Slider.Value);
            NumInput.ValueConfirmed += () =>
                OnValueConfirmed.Invoke(NumInput.Value);

            // Changing the value of the Slider or NumberInput if either of them change
            NumInput.ValueConfirmed += () =>
                Slider.Value = NumInput.Value;
            Slider.ValueChanged += (o, e) =>
                NumInput.Value = Slider.Value;
        }

        public void ChangeRange(double minVal, double maxVal)
        {
            NumInput.Minimum = minVal;
            NumInput.Maximum = maxVal;

            Slider.Minimum = minVal;
            Slider.Maximum = maxVal;
        }
    }
}
