using System;
using System.Collections.Generic;
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
using RPCEventHandler = System.Windows.RoutedPropertyChangedEventHandler<double>;
using RPCEventArgs = System.Windows.RoutedPropertyChangedEventArgs<double>;

namespace MicroscopeGUI.UIElements.Steps
{
    class ConfigStepCon : StepCon
    {
        public ConfigStepCon(Grid Parent, int Row = 1) : base(Parent, Row)
        {
            int RowCount = 0;

            SliderControlInt BrightnessSlider = null;

           UI.Cam.AutoFeatures.Software.Shutter.GetEnable(out bool ShutterEnabled);
            new CheckBoxControl("Auto Shutter", ShutterEnabled,
                new RoutedEventHandler(delegate (object o, RoutedEventArgs a)
                {
                    UI.Cam.AutoFeatures.Software.Shutter.SetEnable((bool)((CheckBox)o).IsChecked);
                }),
            this, RowCount++);


            UI.Cam.AutoFeatures.Software.WhiteBalance.GetEnable(out bool WhiteBalanceEnabled);
            new CheckBoxControl("Auto Whitebalance", WhiteBalanceEnabled,
                new RoutedEventHandler(delegate (object o, RoutedEventArgs a)
                {
                    UI.Cam.AutoFeatures.Software.WhiteBalance.SetEnable((bool)((CheckBox)o).IsChecked);
                }),
            this, RowCount++);


            UI.Cam.AutoFeatures.Software.Gain.GetEnable(out bool GainEnabled);
            new CheckBoxControl("Auto Gain", GainEnabled,
                new RoutedEventHandler(delegate (object o, RoutedEventArgs a)
                {
                    UI.Cam.AutoFeatures.Software.Gain.SetEnable((bool)((CheckBox)o).IsChecked);
                    BrightnessSlider.Enable = (bool)((CheckBox)o).IsChecked;
                }),
            this, RowCount++);


            UI.Cam.Timing.Framerate.GetFrameRateRange(out double FPSMin, out double FPSMax, out _);
            UI.Cam.Timing.Framerate.Get(out double CurrentFPS);
            UpdatingSliderControl FPSSlider = null;
            FPSSlider = new UpdatingSliderControl("FPS", (int)FPSMin, (int)FPSMax, (int)CurrentFPS,
                new RPCEventHandler(delegate (object o, RPCEventArgs a)
                {
                    UI.Cam.Timing.Framerate.Set(((Slider)o).Value);
                }),
                new EventHandler(delegate (object o, EventArgs a)
                {
                    UI.Cam.Timing.Framerate.Get(out double FPS);
                    FPSSlider.SetValue(FPS);
                }),
            this, RowCount++);


            UI.Cam.Gamma.Software.GetRange(out int GammaMin, out int GammaMax, out int GammaInc);
            new SliderControl("Gamma", -1, GammaMax, -1,
                new RPCEventHandler(delegate (object o, RPCEventArgs a)
                {
                    int Val = (int)((Slider)o).Value;
                    if (Val == -1)
                        UI.Cam.Gamma.Software.GetDefault(out Val);
                    UI.Cam.Gamma.Software.Set(Val);
                }),
            this, RowCount++);


            // Only works if Auto Gain is enabled
            BrightnessSlider = new SliderControlInt("Brightness", 0, 255, 128,
                new RPCEventHandler(delegate (object o, RPCEventArgs a)
                {
                    UI.Cam.AutoFeatures.Software.Reference.Set((uint)((Slider)o).Value);
                }),
            this, RowCount++, false);


            UI.Cam.Color.Temperature.GetRange(out uint MinTemp, out uint MaxTemp, out _);
            UI.Cam.Color.Temperature.GetDefault(out uint DefaultTemp);
            new SliderControlInt("Color Temperature", (int)MinTemp, (int)MaxTemp, (int)DefaultTemp,
                new RPCEventHandler(delegate (object o, RPCEventArgs e)
                {
                    UI.Cam.Color.Temperature.Set((uint)((Slider)o).Value);
                }),
            this, RowCount++);


            new SliderControl("Master Gain", 0, 100, 0,
                new RPCEventHandler(delegate (object o, RPCEventArgs a)
                {
                    UI.Cam.AutoFeatures.Software.Gain.SetEnable(false);
                    UI.Cam.Gain.Hardware.ConvertScaledToFactor.Master((int)((Slider)o).Value, out int factor);
                    UI.Cam.Gain.Hardware.Factor.SetMaster(factor, out int newaaaa);
                }),
            this, RowCount++);


            new SliderControlInt("Saturation", 0, 200, 100,
                new RPCEventHandler(delegate (object o, RPCEventArgs a)
                {
                    UI.Cam.Saturation.Set((int)((Slider)o).Value, (int)((Slider)o).Value);
                }),
            this, RowCount++);


            UI.Cam.Timing.Exposure.Get(out double CurrentExposure);
            UI.Cam.Timing.Exposure.GetRange(out double MinExposure, out double MaxExposure, out _);
            new SliderControl("Exposure", MinExposure, MaxExposure, CurrentExposure,
                new RPCEventHandler(delegate (object o, RPCEventArgs a)
                {
                    UI.Cam.Timing.Exposure.Set(((Slider)o).Value);
                }),
            this, RowCount++);
        }
    }
}
