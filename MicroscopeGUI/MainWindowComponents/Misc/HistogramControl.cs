using System;
using ScottPlot;
using std;
using Histogram = peak.ipl.Histogram;

namespace MicroscopeGUI
{
    public class HistogramControl
    {
        // Contain the seperated r g b values from the histogram
        readonly double[] ValuesR = new double[255];
        readonly double[] ValuesG = new double[255];
        readonly double[] ValuesB = new double[255];

        readonly WpfPlot HistogramPlot;

        public HistogramControl(WpfPlot histogramPlot)
        {
            HistogramPlot = histogramPlot;

            // Design for the HistogramPlot
            HistogramPlot.Plot.Frameless();
            HistogramPlot.Plot.SetAxisLimits(0, 255);
            HistogramPlot.Plot.Grid(false);
            HistogramPlot.Plot.Style(Style.Gray2);
            HistogramPlot.MaxWidth = 180;

            CamControl.HistogramUpdated += UpdateHistogram;
        }

        // Seperates the r g b values from the histogram and adds them onto the plot
        public void UpdateHistogram(Histogram histogram)
        {
            HistogramChannelCollection collection = histogram.Channels();

            Array.Copy(collection[0].Bins.ToArray(), 0, ValuesR, 0, 255);
            Array.Copy(collection[1].Bins.ToArray(), 0, ValuesG, 0, 255);
            Array.Copy(collection[2].Bins.ToArray(), 0, ValuesB, 0, 255);

            UI.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                HistogramPlot.Plot.Clear();
                HistogramPlot.Plot.AddBar(ValuesR, System.Drawing.Color.Red).BorderLineWidth = 0;
                HistogramPlot.Plot.AddBar(ValuesG, System.Drawing.Color.Green).BorderLineWidth = 0;
                HistogramPlot.Plot.AddBar(ValuesB, System.Drawing.Color.Blue).BorderLineWidth = 0;
                HistogramPlot.Render();
            }), System.Windows.Threading.DispatcherPriority.Background);
        }
    }
}
