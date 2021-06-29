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
            

            Controls.Add(SnapButton, 0, 0);
        }

        private void SnapButtonClick(object sender, EventArgs e)
        {
            GUI.Camera.Memory.GetLast(out int s32LastMemId);
            GUI.Camera.Memory.Lock(s32LastMemId);
            GUI.Camera.Memory.GetSize(s32LastMemId, out int s32Width, out int s32Height);

            Bitmap MyBitmap;
            GUI.Camera.Memory.ToBitmap(s32LastMemId, out MyBitmap);

            // clone bitmap
            Rectangle cloneRect = new Rectangle(0, 0, s32Width, s32Height);
            System.Drawing.Imaging.PixelFormat format = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
            Bitmap cloneBitmap = MyBitmap.Clone(cloneRect, format);

            // unlock image buffer
            GUI.Camera.Memory.Unlock(s32LastMemId);

            cloneBitmap.Save("C:/Users/Vincent/Desktop/Test.png");
        }
    }
}
