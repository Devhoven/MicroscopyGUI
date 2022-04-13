using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MicroscopeGUI.MainWindowComponents.Controls
{
    class BoolControl : ControlBase
    {
        public delegate void Toggled(bool newVal);

        Toggled OnToggled;

        public BoolControl(string name, bool defaultVal, Toggled onToggled) : base(name)
        {
            OnToggled = onToggled;

            InitControls(defaultVal);
        }

        void InitControls(bool defaultVal)
        {
            CheckBox checkBox = new CheckBox()
            {
                VerticalAlignment = VerticalAlignment.Center,
                IsChecked = defaultVal
            };

            checkBox.Checked += (s, e) 
                => OnToggled(true);
            checkBox.Unchecked += (s, e) 
                => OnToggled(false);

            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto)});
            SetColumn(checkBox, 1);
            Children.Add(checkBox);
        }
    }
}
