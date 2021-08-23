using System;
using System.Windows;
using System.Windows.Controls;

namespace MicroscopeGUI
{
    class ButtonControl : Control
    {
        Button Button;

        public ButtonControl(string Name, RoutedEventHandler ClickEvent, Grid Parent, int Row) : base(Parent)
        {
            Button = new Button();
            Button.Click += ClickEvent;
            Button.Content = Name;

            Parent.Children.Add(Button);
            Grid.SetRow(Button, Row);

            Serializable = false;
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
            CheckBox.Unchecked += BoxClickedEvent;
            CheckBox.Checked += BoxClickedEvent;
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

        protected override string GetName() =>
            (string)Label.Content;

        public override void SetValue(object Value)
            => UI.CurrentDispatcher.Invoke(() => CheckBox.SetValue(CheckBox.IsCheckedProperty, Convert.ToBoolean(Value)));

        public override object GetValue() =>
            CheckBox.IsChecked;
    }
}
