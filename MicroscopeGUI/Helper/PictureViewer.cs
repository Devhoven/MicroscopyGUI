using System.Drawing;
using System.Drawing.Drawing2D;
using System.Numerics;
using System.Windows.Forms;


namespace MicroscopeGUI
{
    public partial class PictureViewer : PictureBox
    {
        new int Width = 1240;
        new int Height = 1024;

        bool Panning = false;

        float ZoomFactor = 1.0f;
        Vector2 Offset;
        Vector2 OldMousePos;
        Vector2 CurrentMousePos;

        public new Image Image
        {
            get { return base.BackgroundImage; }
            set { base.BackgroundImage = value; }
        }

        public PictureViewer()
        {
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            SizeMode = PictureBoxSizeMode.Normal;    

            MouseWheel += PictureViewer_MouseWheel;

            MouseDown += PictureViewer_MouseDown;
            MouseUp += PictureViewer_MouseUp;
            MouseMove += PictureViewer_MouseMove;
        }

        private void PictureViewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (Panning)
            {
                Offset.X += (e.X - OldMousePos.X) / ZoomFactor;
                Offset.Y += (e.Y - OldMousePos.Y) / ZoomFactor;

                OldMousePos = new Vector2(e.X, e.Y);
            }
        }

        private void PictureViewer_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                Panning = false;
        }

        private void PictureViewer_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                Panning = true;

            OldMousePos = new Vector2(e.X, e.Y);
        }

        private void PictureViewer_MouseWheel(object sender, MouseEventArgs e)
        {
            ZoomFactor *= e.Delta > 0 ? 1.1f : 0.9f;
            ZoomFactor = ZoomFactor < 0.8f ? 0.8f : ZoomFactor;

            CurrentMousePos = new Vector2(e.X, e.Y);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics Graphics = pe.Graphics;
            Graphics.Clear(Color.White);

            Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;

            Vector2 Pos = new Vector2(CurrentMousePos.X - Offset.X, CurrentMousePos.Y - Offset.Y);

            Vector2 UVBottom = new Vector2(Width, Height);

            Vector2 TexXY = Pos - Pos * ZoomFactor;
            Vector2 TexXYBottom = UVBottom * ZoomFactor + Pos;

            TexXYBottom -= Pos * ZoomFactor;
            TexXYBottom -= TexXY;

            Graphics.DrawImage(Image, 
                new Rectangle((int)(TexXY.X + Offset.X), (int)(TexXY.Y + Offset.Y), (int)(TexXYBottom.X), (int)(TexXYBottom.Y)),
                new Rectangle(0, 0, Width, Height), GraphicsUnit.Pixel);

            // Forces the picture box to repaint
            Invalidate();
        }
    }
}
