using MicroscopeGUI.MainWindowComponents.Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MicroscopeGUI.MainWindowComponents.ImageViewer.Shader.Classes
{
    class EffectContainer : StackPanel
    {
        public delegate void ChangeValue(float newValue);

        readonly static List<EffContainer> RGB = new List<EffContainer>()
        {
            new EffContainer("Red", 255, 0, 255, (val) =>
            {
                UI.FrameEffects.AmountR = val / 255;
            }),
            new EffContainer("Green", 255, 0, 255, (val) =>
            {
                UI.FrameEffects.AmountG = val / 255;
            }),
            new EffContainer("Blue", 255, 0, 255, (val) =>
            {
                UI.FrameEffects.AmountB = val / 255;
            }),
            new EffContainer("Brightness", 0, -255, 255, (val) =>
            {
                UI.FrameEffects.Brightness = val / 255;
            }),
            new EffContainer("Contrast", 1, 1, 6, (val) =>
            {
                UI.FrameEffects.Contrast = val;
            })
        };

        public struct EffContainer
        {
            public string Name;
            public float StartValue;
            public float Minimum;
            public float Maximum;
            public ChangeValue ChangeValue;

            public EffContainer(string name, float startValue, float minimum, float maximum, ChangeValue changeValue)
            {
                Name = name;
                StartValue = startValue;
                Minimum = minimum;
                Maximum = maximum;
                ChangeValue = changeValue;
            }
        }

        public EffectContainer()
        {
            //Test t = () => EffectControls.InitControls();
            foreach (EffContainer element in RGB)
            {
                Children.Add(new EffectControls(element));
            }
        }

    }
}
