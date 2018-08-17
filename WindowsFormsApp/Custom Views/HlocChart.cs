using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Trading.Analyzers.LegAnalyzer;
using Trading.Common;
using Trading.Analyzers.Common;
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApp
{
    public partial class HlocChart : UserControl
    {

        public HlocChart()
        {
            InitializeComponent();
        }

        public LegAnalyzer LegAnalyzer { get; set; }

        private void Chart1_PostPaint(object sender, ChartPaintEventArgs e)
        {
            if (!DesignMode)
            {
                drawRefLines(e.ChartGraphics.Graphics);
            }
        }

        public void DrawAnalyzer()
        {
            drawBars();
            //populateLines();
        }

        private void drawBars()
        {
            var chartSeries = chart1.Series[0];
 
            foreach (var leg in LegAnalyzer.LegList)
            {
                foreach (var bar in leg.BarList)
                {
                    DataPoint dp = new DataPoint
                    {
                        AxisLabel = bar.DateTime.ToString(),
                        XValue = chartSeries.Points.Count,
                        YValues = new double[] { bar.High, bar.Low, bar.Open, bar.Close },
                        Color = leg.Direction == LegDirection.Up ? Color.Green : Color.Red
                    };

                    chartSeries.Points.Add(dp);
                }
            }
        }

        private void populateLines()
        {
            var refList = LegAnalyzer.RefList;
            var cs2 = chart1.Series[1];
            bool isIntraday = false;
            if (LegAnalyzer.Resolution.TimeFrame == TimeFrame.Hourly ||
                LegAnalyzer.Resolution.TimeFrame == TimeFrame.Minute)
                isIntraday = true;

            cs2.Color = Color.Blue;
            foreach (var r in refList)
            {
                var dp = new DataPoint();
                string dts = isIntraday ? r.DateTime.ToShortTimeString() : r.DateTime.ToShortDateString();
                var d = chart1.Series[0].Points.FirstOrDefault(p => p.AxisLabel.Equals(dts));
                dp.XValue = d.XValue;
                dp.YValues = new double[] { r.Price };

                cs2.Points.Add(dp);
                var lp = chart1.Series[0].Points.Last().XValue;
                cs2.Points.AddXY(lp, dp.YValues[0]);
                //float x1 = (float)chart1.ChartAreas[0].AxisX.ValueToPixelPosition(d.XValue);
                //float x2 = chart1.Right;
                //var y = (float)chart1.ChartAreas[0].AxisY2.ValueToPixelPosition(r.Price);
                //g.DrawLine(new Pen(Color.Red), x1, y, x2, y);
            }
        }

        private void drawRefLines(Graphics g)
        {
            foreach (var r in LegAnalyzer.RefList)
            {
                //string dts = isLaIntraday() ? r.DateTime.ToShortTimeString() : r.DateTime.ToShortDateString();
                string dts = r.DateTime.ToString();
                var d = chart1.Series[0].Points.FirstOrDefault(p => p.AxisLabel.Equals(dts));
                float x1 = (float)chart1.ChartAreas[0].AxisX.ValueToPixelPosition(d.XValue);
                float x2 = (float)chart1.ChartAreas[0].AxisX.ValueToPixelPosition(chart1.Series[0].Points.Count - 1) + 20;
                var y = (float)chart1.ChartAreas[0].AxisY2.ValueToPixelPosition(r.Price);
                g.DrawLine(new Pen(Color.Red), x1, y, x2, y);
            }
        }

        private bool isLaIntraday()
        {
            bool isIntraday = false;
            if (LegAnalyzer.Resolution.TimeFrame == TimeFrame.Hourly ||
                LegAnalyzer.Resolution.TimeFrame == TimeFrame.Minute)
                isIntraday = true;
            return isIntraday;
        }
    }
}
