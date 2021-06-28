using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace MicroscopeGUI
{
    class ToolCon : TableLayoutPanel
    {
        public string Name;

        public ToolCon(string Name)
        {
            this.Name = Name;

        }
    }
}
