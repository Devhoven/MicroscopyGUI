using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MicroscopeGUI
{
    partial class Control
    {
        public Label Label;

        protected bool _Enable;
        protected bool Serializable;

        public virtual bool Enable
        {
            get => _Enable;
            set => _Enable = value;
        }

        protected Control(Grid Parent)
        {
            // Has to be initialized for every new row
            RowDefinition CheckBoxRowDefinition = new RowDefinition()
            {
                Height = GridLength.Auto
            };
            Parent.RowDefinitions.Add(CheckBoxRowDefinition);

            AllControls.Add(this);
            Serializable = true;
        }

        protected Control(string Name, Grid Parent) : this(Parent)
        {
            Label = new Label();
            Label.Foreground = Brushes.White;
            Label.Content = Name;
            Label.VerticalAlignment = VerticalAlignment.Top;
            Label.HorizontalAlignment = HorizontalAlignment.Left;
        }

        // Used for constructing the xml string
        protected virtual string GetName() => "";

        // Sets the value of the given control
        public virtual void SetValue(object Value) { }

        // Returns the value of the given control
        public virtual object GetValue() => null;
    }
}
