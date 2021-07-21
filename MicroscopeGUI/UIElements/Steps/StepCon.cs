using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace MicroscopeGUI.UIElements.Steps
{
    class StepCon : Grid
    {
        public StepCon(Grid Parent, int Row = 1)
        {
            Parent.Children.Add(this);
            Grid.SetRow(this, Row);
        }
    }
}
