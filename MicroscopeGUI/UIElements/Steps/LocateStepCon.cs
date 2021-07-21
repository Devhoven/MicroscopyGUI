using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MicroscopeGUI.UIElements.Steps
{
    class LocateStepCon : StepCon
    {
        bool IsFrozen = false;

        public LocateStepCon(Grid Parent, int Row = 1) : base(Parent, Row)
        {
            Button FreezeBtn = new Button()
            {
                Content = "Freeze"
            };
            FreezeBtn.Click += FreezeBtnClick;
            Children.Add(FreezeBtn);
            SetRow(FreezeBtn, 0);
        }

        private void FreezeBtnClick(object sender, RoutedEventArgs e)
        {
            if (!IsFrozen)
                UI.Cam.Acquisition.Freeze();
            else
                UI.Cam.Acquisition.Capture();
            IsFrozen = !IsFrozen;
        }
    }
}