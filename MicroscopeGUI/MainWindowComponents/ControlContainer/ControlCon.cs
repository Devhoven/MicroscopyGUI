using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace MicroscopeGUI
{
    class ControlCon : Grid
    {
        internal ControlCon(StackPanel Parent, int Row = 1)
        {
            Parent.Children.Add(this);
        }
    }
}
