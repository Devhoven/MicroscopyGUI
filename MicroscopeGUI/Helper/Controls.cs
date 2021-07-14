using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MicroscopeGUI
{
    class Control
    {
        public Label Label;

        protected bool _Enable;

        public virtual bool Enable
        {
            get { return _Enable; }
            set { _Enable = value; }
        }

        public Control(string Name, Grid Parent)
        {
            Label = new Label();
            Label.Content = Name;
            Label.VerticalAlignment = VerticalAlignment.Top;
            Label.HorizontalAlignment = HorizontalAlignment.Left;

            // Has to be initialized for every new row
            RowDefinition CheckBoxRowDefinition = new RowDefinition()
            {
                Height = GridLength.Auto
            };
            Parent.RowDefinitions.Add(CheckBoxRowDefinition);
        }
    }

    class CheckBoxControl : Control
    {
        // Standard margins for the label
        static Thickness LabelMargin = new Thickness()
        {
            Right = 20
        };
        static Thickness CheckBoxMargin = new Thickness()
        {
            Right = 5
        };

        public override bool Enable
        {
            get => CheckBox.IsEnabled;
            set => CheckBox.IsEnabled = value;
        }

        CheckBox CheckBox;

        public CheckBoxControl(string Name, bool DefaultEnabled, RoutedEventHandler BoxClickedEvent,
            Grid Parent, int Row) : base(Name, Parent)
        {
            CheckBox = new CheckBox();
            CheckBox.IsChecked = DefaultEnabled;
            CheckBox.Click += BoxClickedEvent;
            CheckBox.VerticalAlignment = VerticalAlignment.Center;
            CheckBox.HorizontalAlignment = HorizontalAlignment.Right;
            CheckBox.Margin = CheckBoxMargin;

            Label.Margin = LabelMargin;

            // Adding the elements to the parent grid
            Parent.Children.Add(Label);
            Parent.Children.Add(CheckBox);
            Grid.SetRow(Label, Row);
            Grid.SetRow(CheckBox, Row);
        }
    }

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

        string OriginalName;
        public Slider Slider;

        public override bool Enable
        {
            get => Slider.IsEnabled; 
            set => Slider.IsEnabled = value;
        }

        public SliderControl(string Name, int Min, int Max, int StartVal, RoutedPropertyChangedEventHandler<double> ValueChangedEvent,
            Grid Parent, int Row, bool EnabledByDefault = true) : base(Name + " (" + StartVal + "):", Parent)
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
        public void SetValue(double Value)
        {
            UI.CurrentDispatcher.Invoke(() => Slider.Value = Value);
        }

        private void ChangeLabel(object sender, EventArgs e) =>
            Label.Content = OriginalName + " (" + (int)Slider.Value + "): ";
    }

    class UpdatingSliderControl : SliderControl
    {
        public UpdatingSliderControl(string Name, int Min, int Max, int StartVal, RoutedPropertyChangedEventHandler<double> ValueChangedEvent,
            EventHandler OnFrameChange, Grid Parent, int Row, bool EnabledByDefault = true) :
            base(Name, Min, Max, StartVal, ValueChangedEvent, Parent, Row, EnabledByDefault)
        {
            ImageQueue.OnFrameChange += OnFrameChange;
        }
    }
}
