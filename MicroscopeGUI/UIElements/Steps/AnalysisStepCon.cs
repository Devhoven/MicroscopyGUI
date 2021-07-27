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
using ImgQueueMode = MicroscopeGUI.ImageQueue.ImgQueueMode;

namespace MicroscopeGUI.UIElements.Steps
{
    class AnalysisStepCon : StepCon
    {
        public AnalysisStepCon(Grid Parent, int Row = 1) : base(Parent, Row)
        {
            int RowCount = 0;

            new ButtonControl("Freeze", new RoutedEventHandler(delegate (object o, RoutedEventArgs e)
            {
                if (ImageQueue.Mode == ImgQueueMode.Live)
                {
                    UI.Cam.Acquisition.Freeze();
                    ImageQueue.Mode = ImgQueueMode.Frozen;
                    ((Button)o).Content = "Live";
                }
                else
                {
                    UI.Cam.Acquisition.Capture();
                    ImageQueue.Mode = ImgQueueMode.Live;
                    ((Button)o).Content = "Freeze";
                }
            }), this, RowCount++);

            new SliderControl("Contrast", 1, 6, 1, new RPCEventHandler(delegate (object o, RPCEventArgs e)
            {
                UI.FrameEffects.Contrast = (float)((Slider)o).Value;
            }), this, RowCount++);

            new SliderControl("Brightness", -255, 255, 0, new RPCEventHandler(delegate (object o, RPCEventArgs e)
            {
                UI.FrameEffects.Brightness = (float)((Slider)o).Value / 255;
            }), this, RowCount++);

            new SliderControl("Amount of Red", 0, 255, 255, new RPCEventHandler(delegate (object o, RPCEventArgs e)
            {
                UI.FrameEffects.AmountR = (float)((Slider)o).Value / 255;
            }), this, RowCount++);

            new SliderControl("Amount of Green", 0, 255, 255, new RPCEventHandler(delegate (object o, RPCEventArgs e)
            {
                UI.FrameEffects.AmountG = (float)((Slider)o).Value / 255;
            }), this, RowCount++);

            new SliderControl("Amount of Blue", 0, 255, 255, new RPCEventHandler(delegate (object o, RPCEventArgs e)
            {
                UI.FrameEffects.AmountB = (float)((Slider)o).Value / 255;
            }), this, RowCount++);
        }
    }
}
