using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace MicroscopeGUI
{
    public partial class HistogramForm : Form
    {
        Chart HistogramChart;

        delegate void UpdateChartCallback();

        public HistogramForm()
        {
            InitializeComponent();

            HistogramChart = new Chart();
            HistogramChart.ChartAreas.Add("Histogram");
            HistogramChart.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
            HistogramChart.ChartAreas[0].AxisY.MajorGrid.LineWidth = 0;
            HistogramChart.ChartAreas[0].AxisY.MajorTickMark.Enabled = false;
            HistogramChart.ChartAreas[0].AxisY.MinorTickMark.Enabled = false;
            HistogramChart.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.White;

            SeriesChartType Type = SeriesChartType.StackedArea;

            Series Series = HistogramChart.Series.Add("Blue");
            Series.ChartType = Type;
            Series.Color = Color.Blue;
            for (int i = 0; i < 256; i++)
                Series.Points.Add(i);

            Series = HistogramChart.Series.Add("Green");
            Series.ChartType = Type;
            Series.Color = Color.Green;
            for (int i = 0; i < 256; i++)
                Series.Points.Add(i);

            Series = HistogramChart.Series.Add("Red");
            Series.ChartType = Type;
            Series.Color = Color.Red;
            for (int i = 0; i < 256; i++)
                Series.Points.Add(i);

            HistogramChart.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Controls.Add(HistogramChart);

            ImageQueue.OnFrameChange += TimerTick;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            UpdateChart();
        }
        void UpdateChart()
        {
            if (HistogramChart.InvokeRequired)
            {
                UpdateChartCallback Callback = new UpdateChartCallback(UpdateChart);
                BeginInvoke(Callback);
            }
            else
            {
                SeriesChartType Type = SeriesChartType.StackedArea;

                Series Series = HistogramChart.Series["Blue"];
                Series.ChartType = Type;
                for (int i = 0; i < 256; i++)
                    Series.Points[i].YValues[0] = ImageQueue.Histogram[i];

                Series = HistogramChart.Series["Green"];
                Series.ChartType = Type;
                for (int i = 0; i < 256; i++)
                    Series.Points[i].YValues[0] = ImageQueue.Histogram[i + 256];

                Series = HistogramChart.Series["Red"];
                Series.ChartType = Type;
                for (int i = 0; i < 256; i++)
                    Series.Points[i].YValues[0] = ImageQueue.Histogram[i + 512];

                HistogramChart.Size = new Size(Size.Width - 25, Size.Height - 40);
            }
        }

        private void HistogramForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ImageQueue.OnFrameChange -= TimerTick;
        }
    }
}
