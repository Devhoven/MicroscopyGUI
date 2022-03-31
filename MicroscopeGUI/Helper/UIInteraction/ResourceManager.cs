using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MicroscopeGUI.Helper.UIInteraction
{
    static class ResourceManager
    {
        public static object GetResource(string name)
            => Application.Current.FindResource(name);
    }
}
