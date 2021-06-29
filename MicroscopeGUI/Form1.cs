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

        StepCon[] Tools;
        int CurrentTool;

        public GUI()
        {
            InitializeComponent();

            Camera.Init();
            DisplayHandle = LiveImgCon.Handle;
            Camera.Size.AOI.Get(out Rectangle CameraRect);
            LiveImgCon.MaximumSize = CameraRect.Size;

            Camera.Memory.Allocate();

            Camera.EventFrame += CameraEventFrame;

            Camera.Acquisition.Capture();

            Tools = new StepCon[] { new ConfigStepCon(), new LocateStepCon() };
            CurrentToolCon.Controls.Add(Tools[0]);
            SetLabelText();
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
            CurrentToolCon.Controls.Remove(Tools[CurrentTool]);
            CurrentTool = Math.Min(CurrentTool + 1, Tools.Length - 1);
            CurrentToolCon.Controls.Add(Tools[CurrentTool], 0, 2);

            SetLabelText();
        }

        void SetLabelText()
        {
            if (CurrentTool - 1 >= 0)
                PreviousStepLabel.Text = Tools[CurrentTool - 1].Name;
            else
                PreviousStepLabel.Text = "";

            if (CurrentTool + 1 <= Tools.Length - 1)
                NextStepLabel.Text = Tools[CurrentTool + 1].Name;
            else
                NextStepLabel.Text = "";

            StepLabel.Text = Tools[CurrentTool].Name;
        }

        private void BackBtnClick(object sender, EventArgs e)
        {
            CurrentToolCon.Controls.Remove(Tools[CurrentTool]);
            CurrentTool = Math.Max(CurrentTool - 1, 0);
            CurrentToolCon.Controls.Add(Tools[CurrentTool], 0, 2);

            SetLabelText();
        }
    }
}
