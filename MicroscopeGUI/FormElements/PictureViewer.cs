using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;


namespace MicroscopeGUI
{
    public partial class PictureViewer : PictureBox
    {
        int Width = 1240;
        int Height = 1024;

        float ZoomFactor = 1.0f;

        bool Panning = false;
        Point OldMousePos;
        Point Offset;

        public new Image Image
        {
            get { return base.BackgroundImage; }
            set { base.BackgroundImage = value; }
        }

        public PictureViewer()
        {
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            MouseWheel += PictureViewer_MouseWheel;

            MouseDown += PictureViewer_MouseDown;
            MouseUp += PictureViewer_MouseUp;
            MouseMove += PictureViewer_MouseMove;
        }

        private void PictureViewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (Panning)
            {
                Offset = new Point(e.Location.X - OldMousePos.X, e.Location.Y - OldMousePos.Y);
            }
        }

        private void PictureViewer_MouseUp(object sender, MouseEventArgs e)
        {
            Panning = false;
        }

        private void PictureViewer_MouseDown(object sender, MouseEventArgs e)
        {
            Panning = true;
            OldMousePos = new Point(e.Location.X - Offset.X, e.Location.Y - Offset.Y);
        }

        private void PictureViewer_MouseWheel(object sender, MouseEventArgs e)
        {
            ZoomFactor += e.Delta > 0 ? 0.2f : -0.2f;
            ZoomFactor = ZoomFactor < 1 ? 1 : ZoomFactor;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics Graphics = pe.Graphics;

            Graphics.Clear(Color.White);

            Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            int ZoomedWidth = (int)(Width * ZoomFactor);
            int ZoomedHeight = (int)(Height * ZoomFactor);
            int OffsetX = (base.Width - ZoomedWidth) / 2 + Offset.X;
            int OffsetY = (base.Height - ZoomedHeight) / 2 + Offset.Y;
            Graphics.DrawImage(Image, 
                new Rectangle(OffsetX, OffsetY, ZoomedWidth, ZoomedHeight), 
                new Rectangle(0, 0, Width, Height), GraphicsUnit.Pixel);
        }
    }
}
