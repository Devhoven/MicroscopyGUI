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

            int RowCount = 1;

            SliderControl BrightnessSlider = null;

            GUI.Camera.AutoFeatures.Software.Shutter.GetEnable(out bool ShutterEnabled);
            new CheckBoxControl("Auto Shutter:", ShutterEnabled, new EventHandler(delegate (object o, EventArgs a)
            {
                GUI.Camera.AutoFeatures.Software.Shutter.SetEnable(((CheckBox)o).Checked);
            }),
            Controls, 0, RowCount++);


            GUI.Camera.AutoFeatures.Software.WhiteBalance.GetEnable(out bool WhiteBalanceEnabled);
            new CheckBoxControl("Auto Whitebalance:", WhiteBalanceEnabled, new EventHandler(delegate (object o, EventArgs a)
            {
                GUI.Camera.AutoFeatures.Software.WhiteBalance.SetEnable(((CheckBox)o).Checked);
            }),
            Controls, 0, RowCount++);


            GUI.Camera.AutoFeatures.Software.Gain.GetEnable(out bool GainEnabled);
            new CheckBoxControl("Auto Gain:", GainEnabled, new EventHandler(delegate (object o, EventArgs a)
            {
                GUI.Camera.AutoFeatures.Software.Gain.SetEnable(((CheckBox)o).Checked);
                BrightnessSlider.Enable = ((CheckBox)o).Checked;
            }),
            Controls, 0, RowCount++);


            GUI.Camera.Timing.Framerate.GetFrameRateRange(out double FPSMin, out double FPSMax, out _);
            GUI.Camera.Timing.Framerate.Get(out double CurrentFPS);
            UpdatingSliderControl FPSSlider = null;
            FPSSlider = new UpdatingSliderControl("FPS", (int)FPSMin, (int)FPSMax, (int)CurrentFPS, new EventHandler(delegate (object o, EventArgs a)
            {
                GUI.Camera.Timing.Framerate.Set(((TrackBar)o).Value);
            }),
            new EventHandler(delegate (object o, EventArgs a)
            {
                GUI.Camera.Timing.Framerate.Get(out double FPS);
                FPSSlider.SetValue((int)FPS);
            }),
            Controls, 0, RowCount++);


            GUI.Camera.Gamma.Software.GetRange(out int GammaMin, out int GammaMax, out int GammaInc);
            new SliderControl("Gamma", -1, GammaMax, -1, new EventHandler(delegate (object o, EventArgs a)
            {
                int Val = ((TrackBar)o).Value;
                if (Val == -1)
                    GUI.Camera.Gamma.Software.GetDefault(out Val);
                GUI.Camera.Gamma.Software.Set(Val);
            }),
            Controls, 0, RowCount++);


            // Only works if Auto Gain is enabled
            BrightnessSlider = new SliderControl("Brightness", 0, 255, 128,
            new EventHandler(delegate (object o, EventArgs a)
            {
                GUI.Camera.AutoFeatures.Software.Reference.Set((uint)((TrackBar)o).Value);
            }),
            Controls, 0, RowCount++, false);


            GUI.Camera.Color.Temperature.GetRange(out uint MinTemp, out uint MaxTemp, out _);
            GUI.Camera.Color.Temperature.GetDefault(out uint DefaultTemp);
            new SliderControl("Color Temperature", (int)MinTemp, (int)MaxTemp, (int)DefaultTemp, 
                new EventHandler(delegate (object o, EventArgs e)
            {
                GUI.Camera.Color.Temperature.Set((uint)((TrackBar)o).Value);
            }),
            Controls, 0, RowCount++);


            new SliderControl("Master Gain", 0, 100, 0, new EventHandler(delegate (object o, EventArgs a)
            {
                GUI.Camera.AutoFeatures.Software.Gain.SetEnable(false);
                GUI.Camera.Gain.Hardware.ConvertScaledToFactor.Master(((TrackBar)o).Value, out int factor);
                GUI.Camera.Gain.Hardware.Factor.SetMaster(factor, out int newaaaa);
            }),
            Controls, 0, RowCount++);


            new SliderControl("Saturation", 0, 200, 100, new EventHandler(delegate (object o, EventArgs a)
            {
                GUI.Camera.Saturation.Set(((TrackBar)o).Value, ((TrackBar)o).Value);
            }),
            Controls, 0, RowCount++);
        }
    }
}
