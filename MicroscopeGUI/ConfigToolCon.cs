using System;
using uEye;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using uEye.Types;

namespace MicroscopeGUI
{
    class ConfigToolCon : ToolCon
    {
        Label BrightnessLabel;
        TrackBar BrightnessTrackBar;

        Label ContrastLabel;
        TrackBar ContrastTrackBar;

        public ConfigToolCon(string Name) : base(Name)
        {
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            BrightnessLabel = new Label();
            BrightnessLabel.Text = "Brightness (0):";
            BrightnessTrackBar = new TrackBar();
            BrightnessTrackBar.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            BrightnessTrackBar.Maximum = 255;
            BrightnessTrackBar.Minimum = 0;
            BrightnessTrackBar.ValueChanged += BrightnessTrackBarValueChanged;
            // TODO: Retreive the actual brightness value of the camera
            BrightnessTrackBar.Value = 0;
            Controls.Add(BrightnessTrackBar, 0, 0);
            Controls.Add(BrightnessLabel, 0, 0);

            ContrastLabel = new Label();
            ContrastLabel.Text = "Contrast (0):";
            ContrastTrackBar = new TrackBar();
            ContrastTrackBar.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            ContrastTrackBar.Maximum = 255;
            ContrastTrackBar.Minimum = 0;
            ContrastTrackBar.ValueChanged += ContrastTrackBarValueChanged;
            // TODO: Same as with the brightness
            ContrastTrackBar.Value = 0;
            Controls.Add(ContrastTrackBar, 0, 1);
            Controls.Add(ContrastLabel, 0, 1);
        }

        // TODO: Set the brightness etc. of the camera
        private void BrightnessTrackBarValueChanged(object sender, EventArgs e)
        {
            BrightnessLabel.Text = "Brightness (" + BrightnessTrackBar.Value + "):";
            Range<uint> Range = new Range<uint>();
            GUI.Camera.Color.Temperature.Set((uint)(2350 + (10000 / 255) * BrightnessTrackBar.Value));
        }

        private void ContrastTrackBarValueChanged(object sender, EventArgs e)
        {
            ContrastLabel.Text = "Contrast (" + ContrastTrackBar.Value + "):";
        }
    }
}
