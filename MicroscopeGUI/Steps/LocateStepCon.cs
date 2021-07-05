using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace MicroscopeGUI
{
    class LocateStepCon : StepCon
    {
        Button SnapButton;

        public LocateStepCon() : base("Locate")
        {
            SnapButton = new Button();
            SnapButton.Text = "Snap";
            SnapButton.Click += SnapButtonClick;
            SnapButton.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            Controls.Add(SnapButton, 0, 2);
        }

        private void SnapButtonClick(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "Image|*.png";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ImageQueue.CurrentBitmap.Save(saveFileDialog1.FileName);
            }
        }
    }
}
