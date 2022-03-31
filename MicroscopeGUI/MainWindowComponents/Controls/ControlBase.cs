using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MicroscopeGUI
{
    partial class Control
    {
        public Label Label;

        protected ControlCon Parent;

        protected bool Serializable;

        public virtual bool Enable
        {
            get;
            set;
        }

        protected Control(ControlCon parent)
        {
            Parent = parent;

            AllControls.Add(this);
            Serializable = true;
        }

        protected Control(string name, ControlCon parent) : this(parent)
        {
            Label = new Label()
            {
                Foreground = Brushes.White,
                Content = name,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left
            };
        }

        // Used for constructing the xml string
        protected virtual string GetName() => "";

        // Sets the value of the given control
        public virtual void SetValue(object value) { }

        // Returns the value of the given control
        public virtual object GetValue() => null;
    }
}
