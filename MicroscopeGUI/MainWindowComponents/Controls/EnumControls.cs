using peak.core.nodes;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MicroscopeGUI
{
    class EnumControl : Control
    {
        public EnumControl(EnumerationNode node, ControlCon Parent) : base(node.DisplayName(), Parent)
        {

        }
    }
}
