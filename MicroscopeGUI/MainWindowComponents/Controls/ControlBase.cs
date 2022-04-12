using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MicroscopeGUI
{
    class NodeControl : Grid
    {
        public Label Label;

        public virtual bool Enable
        {
            get;
            set;
        }

        protected NodeControl(string name)
        {
            Label = new Label()
            {
                Foreground = Brushes.White,
                Content = name,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            // The label is always in it's own row at the top
            RowDefinitions.Add(new RowDefinition());
            SetRow(Label, 0);
            SetColumn(Label, 0);
            SetColumnSpan(Label, 2);
            Children.Add(Label);
        }
    }
}
