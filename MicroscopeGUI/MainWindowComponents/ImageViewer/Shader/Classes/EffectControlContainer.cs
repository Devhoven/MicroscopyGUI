using MicroscopeGUI.MainWindowComponents.Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MicroscopeGUI.MainWindowComponents.ImageViewer.Shader.Classes
{
    class EffectControlContainer : StackPanel
    {
        readonly static List<EffectInfo> RGB = new List<EffectInfo>()
        {
            new EffectInfo("Red", 255, 0, 255, (val) =>
            {
                UI.FrameEffects.AmountR = val / 255;
            }),
            new EffectInfo("Green", 255, 0, 255, (val) =>
            {
                UI.FrameEffects.AmountG = val / 255;
            }),
            new EffectInfo("Blue", 255, 0, 255, (val) =>
            {
                UI.FrameEffects.AmountB = val / 255;
            }),
            new EffectInfo("Brightness", 0, -255, 255, (val) =>
            {
                UI.FrameEffects.Brightness = val / 255;
            }),
            new EffectInfo("Contrast", 1, 1, 6, (val) =>
            {
                UI.FrameEffects.Contrast = val;
            })
        };

        public struct EffectInfo
        {
            public string Name;
            public float StartValue;
            public float Minimum;
            public float Maximum;
            public EffectControl.ChangeEffectValue ChangeValue;

            public EffectInfo(string name, float startValue, float minimum, float maximum, EffectControl.ChangeEffectValue changeValue)
            {
                Name = name;
                StartValue = startValue;
                Minimum = minimum;
                Maximum = maximum;
                ChangeValue = changeValue;
            }
        }

        public EffectControlContainer()
        {
            foreach (EffectInfo element in RGB)
                Children.Add(new EffectControl(element));
        }
    }
}
