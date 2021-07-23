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
using P = MicroscopeGUI.Properties.Settings;

namespace MicroscopeGUI.UIElements.Steps
{
    class ConfigStepCon : StepCon
    {
        public ConfigStepCon(Grid Parent, int Row = 1) : base(Parent, Row)
        {
            int RowCount = 0;

            SliderControl BrightnessSlider = null;

            // All of these constructs add another control to the current table

            new CheckBoxControl("Auto Shutter:", P.Default.ShutterEnable,
                new RoutedEventHandler(delegate (object o, RoutedEventArgs a)
                {
                    UI.Cam.AutoFeatures.Software.Shutter.SetEnable((bool)((CheckBox)o).IsChecked);
                }),
            this, RowCount++);


            new CheckBoxControl("Auto Whitebalance:", P.Default.WhiteBalanceEnable,
                new RoutedEventHandler(delegate (object o, RoutedEventArgs a)
                {
                    UI.Cam.AutoFeatures.Software.WhiteBalance.SetEnable((bool)((CheckBox)o).IsChecked);
                }),
            this, RowCount++);


            new CheckBoxControl("Auto Gain:", P.Default.GainEnable,
                new RoutedEventHandler(delegate (object o, RoutedEventArgs a)
                {
                    UI.Cam.AutoFeatures.Software.Gain.SetEnable((bool)((CheckBox)o).IsChecked);
                    BrightnessSlider.Enable = (bool)((CheckBox)o).IsChecked;
                }),
            this, RowCount++);


            UpdatingSliderControl FPSSlider = null;
            FPSSlider = new UpdatingSliderControl("FPS", (int)P.Default.FrameRateMin, (int)P.Default.FrameRateMax, (int)P.Default.CurrentFramerate,
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


            new SliderControl("Gamma", -1, P.Default.GammaMax, -1,
                new RPCEventHandler(delegate (object o, RPCEventArgs a)
                {
                    int Val = (int)((Slider)o).Value;
                    if (Val == -1)
                        UI.Cam.Gamma.Software.Set(P.Default.GammaDefault);
                }),
            this, RowCount++);


            // Only works if Auto Gain is enabled
            BrightnessSlider = new SliderControl("Brightness", 0, 255, 128,
                new RPCEventHandler(delegate (object o, RPCEventArgs a)
                {
                    UI.Cam.AutoFeatures.Software.Reference.Set((uint)((Slider)o).Value);
                }),
            this, RowCount++, false);


            new SliderControl("Color Temperature", (int)P.Default.ColorTempMin, (int)P.Default.ColorTempMax, (int)P.Default.ColorTempDefault,
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


            new SliderControl("Saturation", 0, 200, 100,
                new RPCEventHandler(delegate (object o, RPCEventArgs a)
                {
                    UI.Cam.Saturation.Set((int)((Slider)o).Value, (int)((Slider)o).Value);
                }),
            this, RowCount++);


            new SliderControl("Exposure", (int)P.Default.ExposureMin, (int)P.Default.ExposureMax, (int)P.Default.CurrentExposure,
                new RPCEventHandler(delegate (object o, RPCEventArgs a)
                {
                    UI.Cam.Timing.Exposure.Set(((Slider)o).Value);
                }),
            this, RowCount++);
        }
    }
}
