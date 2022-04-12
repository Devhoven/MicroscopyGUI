using MicroscopeGUI.MainWindowComponents.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MicroscopeGUI.MainWindowComponents.ImageViewer.Shader.Classes
{
    class EffectControl : ControlBase
    {
        RangeInput ValInput;

        public delegate void ChangeEffectValue(float newValue);

        ChangeEffectValue ChangeValue;

        public EffectControl(EffectControlContainer.EffectInfo effect) : base(effect.Name)
        {
            RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            ChangeValue = effect.ChangeValue;

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
