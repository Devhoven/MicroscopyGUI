using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace MicroscopeGUI
{
    class StepCon : TableLayoutPanel
    {
        public string Name;

        public StepCon(string Name)
        {
            this.Name = Name;
        }
    }
}
