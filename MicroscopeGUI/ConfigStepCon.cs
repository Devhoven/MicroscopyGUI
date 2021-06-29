using uEye;
using System;
using uEye.Defines.Whitebalance;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using uEye.Types;

namespace MicroscopeGUI
{
    class ConfigStepCon : StepCon
    {
        public ConfigStepCon() : base("Config")
        {
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            GUI.Camera.Gamma.Software.GetRange(out int GammaMin, out int GammaMax, out int GammaInc);
            new SliderControl("Gamma", -1, GammaMax, -1, new EventHandler(delegate (object o, EventArgs a)
            {
                int Val = ((TrackBar)o).Value;
                if (Val == -1)
                {
                    GUI.Camera.Gamma.Software.GetDefault(out Val);
                }
                GUI.Camera.Gamma.Software.Set(Val);
            }),
            Controls, 0, 1);

            GUI.Camera.Timing.Framerate.GetFrameRateRange(out double FPSMin, out double FPSMax, out _);
            new SliderControl("FPS", (int)FPSMin, (int)FPSMax, (int)FPSMax, new EventHandler(delegate (object o, EventArgs a)
            {
                GUI.Camera.Timing.Framerate.Set(((TrackBar)o).Value);
            }),
            Controls, 0, 2);

            GUI.Camera.AutoFeatures.Software.Shutter.GetEnable(out bool ShutterEnabled);
            new CheckBoxControl("Auto Shutter:", ShutterEnabled, new EventHandler(delegate (object o, EventArgs a)
            {
                GUI.Camera.AutoFeatures.Software.Shutter.SetEnable(((CheckBox)o).Checked);
            }),
            Controls, 0, 3);

            GUI.Camera.AutoFeatures.Software.WhiteBalance.GetEnable(out bool WhiteBalanceEnabled);
            new CheckBoxControl("Auto Whitebalance:", WhiteBalanceEnabled, new EventHandler(delegate (object o, EventArgs a)
            {
                GUI.Camera.AutoFeatures.Software.WhiteBalance.SetEnable(((CheckBox)o).Checked);
            }),
            Controls, 0, 4);

            GUI.Camera.AutoFeatures.Software.Gain.GetEnable(out bool GainEnabled);
            new CheckBoxControl("Auto Gain:", GainEnabled, new EventHandler(delegate (object o, EventArgs a)
            {
                GUI.Camera.AutoFeatures.Software.Gain.SetEnable(((CheckBox)o).Checked);
            }),
            Controls, 0, 5);

        }
    }
}
