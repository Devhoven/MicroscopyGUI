﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MicroscopeGUI
{
    // Slider controls with double values
    class SliderControl : Control
    {
        // Standard margins for the elements
        static Thickness LabelMargin = new Thickness()
        {
            Bottom = 20
        };
        static Thickness SliderMargin = new Thickness()
        {
            Right = 5,
            Left = 5
        };

        internal string OriginalName;
        public Slider Slider;

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

        public SliderControl(string Name, double Min, double Max, double StartVal, RoutedPropertyChangedEventHandler<double> ValueChangedEvent,
            Grid Parent, int Row, bool EnabledByDefault = true) : base(Name + " (" + Math.Round(StartVal, 3) + "):", Parent)
        {
            Slider = new Slider();
            Slider.VerticalAlignment = VerticalAlignment.Bottom;
            Slider.Minimum = Min;
            Slider.Maximum = Max;
            Slider.Value = StartVal;
            Slider.Margin = SliderMargin;

            Slider.ValueChanged += ValueChangedEvent;
            Slider.ValueChanged += ChangeLabel;

            Label.Margin = LabelMargin;

            OriginalName = Name;

            // Adding the elements to the parent grid
            Parent.Children.Add(Label);
            Parent.Children.Add(Slider);
            // Setting the row value 
            Grid.SetRow(Label, Row);
            Grid.SetRow(Slider, Row);

            Enable = EnabledByDefault;
        }

        // Thread safe value setting
        public override void SetValue(object Value) =>
            UI.CurrentDispatcher.Invoke(() => Slider.Value = Convert.ToDouble(Value));

        internal virtual void ChangeLabel(object sender, EventArgs e) =>
            Label.Content = OriginalName + " (" + Math.Round(Slider.Value, 3) + "): ";

        protected override string GetName() =>
            OriginalName;

        public override object GetValue() =>
            Slider.Value;
    }

    // Slider control for ints
    class SliderControlInt : SliderControl
    {
        public SliderControlInt(string Name, int Min, int Max, int StartVal, RoutedPropertyChangedEventHandler<double> ValueChangedEvent,
            Grid Parent, int Row, bool EnabledByDefault = true) : base(Name, Min, Max, StartVal, ValueChangedEvent, Parent, Row, EnabledByDefault)
        {
            // Makes the slider move in integer steps
            Slider.IsSnapToTickEnabled = true;
            Slider.TickFrequency = 1;
        }

        public override object GetValue() =>
            Convert.ToInt32(base.GetValue());

        internal override void ChangeLabel(object sender, EventArgs e) =>
            Label.Content = OriginalName + " (" + (int)Slider.Value + "): ";
    }

    class UpdatingSliderControl : SliderControl
    {
        public UpdatingSliderControl(string Name, double Min, double Max, double StartVal, double Increment, RoutedPropertyChangedEventHandler<double> ValueChangedEvent,
            EventHandler OnFrameChange, Grid Parent, int Row, bool EnabledByDefault = true) :
            base(Name, Min, Max, StartVal, ValueChangedEvent, Parent, Row, EnabledByDefault)
        {
            Slider.TickFrequency = Increment;
            Slider.IsSnapToTickEnabled = true;
            ImageQueue.OnFrameChange += OnFrameChange;
        }
    }
}
