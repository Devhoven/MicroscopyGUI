using System;
using ScottPlot;

namespace MicroscopeGUI
{
    public class HistogramControl
    {
        // Contain the seperated r g b values from the histogram
        double[] ValuesR = new double[255];
        double[] ValuesG = new double[255];
        double[] ValuesB = new double[255];

        WpfPlot HistogramPlot;
        public HistogramControl(WpfPlot HistogramPlot)
        {
            this.HistogramPlot = HistogramPlot;

            // Design for the HistogramPlot
            this.HistogramPlot.Plot.Frameless();
            this.HistogramPlot.Plot.SetAxisLimits(0, 255);
            this.HistogramPlot.Plot.Grid(false);
            this.HistogramPlot.Plot.Frameless();
            this.HistogramPlot.Plot.Style(ScottPlot.Style.Gray2);
        }

        // Seperates the r g b values from the histogram and adds them onto the plot
        public void UpdateHistogram()
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
            HistogramPlot.Render();
        }
    }
}
