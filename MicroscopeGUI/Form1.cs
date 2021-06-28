using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using uEye;
using uEye.Defines;

namespace MicroscopeGUI
{
    public partial class GUI : Form
    {
        public static Camera Camera = new Camera();
        IntPtr DisplayHandle = IntPtr.Zero;


        public GUI()
        {
            InitializeComponent();
            DisplayHandle = LiveImgCon.Handle;

            Camera.Init();

            Camera.Memory.Allocate();

            Camera.EventFrame += CameraEventFrame;

            Camera.Acquisition.Capture();

            CurrentToolCon.Controls.Add(new ConfigToolCon("Hehe"), 0, 2);
        }

        private void CameraEventFrame(object sender, EventArgs e)
        {
            Camera.Display.Render(DisplayHandle, DisplayRenderMode.FitToWindow);
        }

        private void GUIClosing(object sender, FormClosingEventArgs e)
        {
            Camera.Exit();
        }

        private void NextBtnClick(object sender, EventArgs e)
        {

        }

        private void BackBtnClick(object sender, EventArgs e)
        {

        }

    }
}
