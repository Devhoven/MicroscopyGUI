using MicroscopeGUI.MainWindowComponents.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MicroscopeGUI.MainWindowComponents.ImageViewer.Shader.Classes
{
    class EffectControls : Grid
    {
        static Thickness LabelMargin = new Thickness(0, 5, 0, 5);

        RangeInput ValInput;
        Label Label;

        EffectContainer.ChangeValue ChangeValue;

        public EffectControls(EffectContainer.EffContainer effect) 
        {
            RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            ChangeValue = effect.ChangeValue;

            Label = new Label()
            {
                Foreground = Brushes.White,
                Content = effect.Name,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = LabelMargin
            };

            SetRow(Label, 0);
            Children.Add(Label);

            InitControls(effect.StartValue, effect.Minimum, effect.Maximum);
        }

        public void InitControls(float startVal, float minVal, float maxVal)
        {
            ValInput = new RangeInput(startVal, minVal, maxVal, 1);

            SetColumn(ValInput, 1);
            SetRow(ValInput, 1);
            Children.Add(ValInput);

            ValInput.OnValueChanged += SliderValueChanged;
            ValInput.OnValueConfirmed += SliderValueChanged;
        }

        private void SliderValueChanged(double newVal)
            => ChangeValue((float)newVal);
    }
}
