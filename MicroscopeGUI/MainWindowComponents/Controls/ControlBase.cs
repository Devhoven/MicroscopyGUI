﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MicroscopeGUI.MainWindowComponents.Controls
{
    abstract class ControlBase : Grid
    {
        // Standard margins for the label
        static readonly Thickness LABEL_MARGIN = new Thickness(0, 5, 0, 5);

        public Label Label;

        public virtual bool Enable
        {
            get;
            set;
        }

        protected ControlBase(string name)
        {
            Label = new Label()
            {
                Foreground = Brushes.White,
                Content = name,
                Margin = LABEL_MARGIN,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            // The label is always in it's own row at the top
            RowDefinitions.Add(new RowDefinition());
            ColumnDefinitions.Add(new ColumnDefinition());
            SetRow(Label, 0);
            SetColumn(Label, 0);
            Children.Add(Label);
        }
    }
}
