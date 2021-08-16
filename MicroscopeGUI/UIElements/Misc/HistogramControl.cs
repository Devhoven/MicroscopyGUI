using ScottPlot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace MicroscopeGUI
{
    class HistogramControl
    {
        // Contain the seperated r g b values from the histogram
        double[] ValuesR = new double[255];
        double[] ValuesG = new double[255];
        double[] ValuesB = new double[255];

        // Calls an event in the interval that is given to it
        DispatcherTimer UpdateTimer;

        WpfPlot HistogramPlot;
        public HistogramControl(WpfPlot HistogramPlot)
        {
            this.HistogramPlot = HistogramPlot;

            // Setting the interval and starting the Timer
            UpdateTimer = new DispatcherTimer();
            UpdateTimer.Interval = new TimeSpan(500000);
            UpdateTimer.Tick += ImageQueueFrameChange;
            UpdateTimer.Start();
        }

        // Seperates the r g b values from the histogram and adds them onto the plot
        private void ImageQueueFrameChange(object sender, EventArgs e)
        {
            Array.Copy(ImageQueue.Histogram, 0, ValuesR, 0, 255);

            // Starts at 256, since the value at 255 does not represent a real value
            Array.Copy(ImageQueue.Histogram, 256, ValuesG, 0, 255);

            // Same here
            Array.Copy(ImageQueue.Histogram, 512, ValuesB, 0, 255);

            HistogramPlot.Plot.Clear();
            HistogramPlot.Plot.AddBar(ValuesR, System.Drawing.Color.Red).BorderLineWidth = 0;
            HistogramPlot.Plot.AddBar(ValuesG, System.Drawing.Color.Green).BorderLineWidth = 0;
            HistogramPlot.Plot.AddBar(ValuesB, System.Drawing.Color.Blue).BorderLineWidth = 0;
            HistogramPlot.Plot.SetAxisLimits(0, 255);
            HistogramPlot.Plot.Frameless();
            HistogramPlot.Render();
        }
    }
}
