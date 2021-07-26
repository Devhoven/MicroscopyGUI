using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace MicroscopeGUI
{
    public class EffectShader : ShaderEffect
    {
        static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty("Frame", typeof(EffectShader), 0);

        static readonly DependencyProperty BrightnessProperty = DependencyProperty.Register("Brightness", typeof(float), typeof(EffectShader),
            new UIPropertyMetadata(0.0f, PixelShaderConstantCallback(0)));
        static readonly DependencyProperty ContrastProperty = DependencyProperty.Register("Contrast", typeof(float), typeof(EffectShader),
            new UIPropertyMetadata(1.0f, PixelShaderConstantCallback(1)));

        static readonly DependencyProperty AmountRProperty = DependencyProperty.Register("AmountR", typeof(float), typeof(EffectShader),
            new UIPropertyMetadata(1.0f, PixelShaderConstantCallback(2)));
        static readonly DependencyProperty AmountGProperty = DependencyProperty.Register("AmountG", typeof(float), typeof(EffectShader),
            new UIPropertyMetadata(1.0f, PixelShaderConstantCallback(3)));
        static readonly DependencyProperty AmountBProperty = DependencyProperty.Register("AmountB", typeof(float), typeof(EffectShader),
            new UIPropertyMetadata(1.0f, PixelShaderConstantCallback(4)));

        public Brush Input
        {
            get => (Brush)GetValue(InputProperty);
            set => SetValue(InputProperty, value);
        }
        public float Brightness
        {
            get => (float)GetValue(BrightnessProperty);
            set => SetValue(BrightnessProperty, value);
        }
        public float Contrast
        {
            get => (float)GetValue(ContrastProperty);
            set => SetValue(ContrastProperty, value);
        }
        public float AmountR
        {
            get => (float)GetValue(AmountRProperty);
            set => SetValue(AmountRProperty, value);
        }
        public float AmountG
        {
            get => (float)GetValue(AmountGProperty);
            set => SetValue(AmountGProperty, value);
        }
        public float AmountB
        {
            get => (float)GetValue(AmountBProperty);
            set => SetValue(AmountBProperty, value);
        }

        public EffectShader()
        {
            PixelShader pixelShader = new PixelShader();

            pixelShader.UriSource = new Uri("pack://application:,,,/Shader/Compiled/EffectShader.ps", UriKind.Absolute);

            PixelShader = pixelShader;
            UpdateShaderValue(InputProperty);
            UpdateShaderValue(BrightnessProperty);
            UpdateShaderValue(ContrastProperty);
            UpdateShaderValue(AmountRProperty);
            UpdateShaderValue(AmountGProperty);
            UpdateShaderValue(AmountBProperty);
        }
    }
}
