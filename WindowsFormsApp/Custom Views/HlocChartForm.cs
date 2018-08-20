using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Trading.Analyzers.Common;
using Trading.Analyzers.LegAnalyzer;
using Trading.Common;

namespace WindowsFormsApp.Custom_Views
{
    public partial class HlocChartForm : Form
    {
        public HlocChartForm()
        {
            InitializeComponent();
        }

        public LegAnalyzer LegAnalyzer { get; set; }


        public void DrawAnalyzer()
        {
            drawBars();
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
            //chart1.ChartAreas[0].AxisY2.

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
                
                //float x1 = (float)hlocChartControl1.ChartAreas[0].AxisX.ValueToPixelPosition(d.XValue);
                //float x2 = hlocChartControl1.Right;
                //var y = (float)hlocChartControl1.ChartAreas[0].AxisY2.ValueToPixelPosition(r.Price);
                //g.DrawLine(new Pen(Color.Red), x1, y, x2, y);
            }
        }

        private void drawRefLines(Graphics g)
        {
            foreach (var r in LegAnalyzer.RefList)
            {
                string dts = r.DateTime.ToString();
                var d = chart1.Series[0].Points.FirstOrDefault(p => p.AxisLabel.Equals(dts));
                //this.BringToFront();

                float x1 = (float)chart1.ChartAreas[0].AxisX.ValueToPixelPosition(d.XValue);
                float x2 = (float)chart1.ChartAreas[0].AxisX.ValueToPixelPosition(chart1.Series[0].Points.Count - 1) + 20;
                var y = (float)chart1.ChartAreas[0].AxisY2.ValueToPixelPosition(r.Price);
                g.DrawLine(new Pen(Color.Black, 1), x1, y, x2, y);

                var font = new Font(FontFamily.GenericSerif,8);
                SolidBrush drawBrush = new SolidBrush(Color.Black);
                g.DrawString("" + r.Price, font, drawBrush, x2 + 7, y-10);
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

        private void HlocChartForm_Paint(object sender, PaintEventArgs e)
        {

        }

        private void chart1_PostPaint(object sender, ChartPaintEventArgs e)
        {
            drawRefLines(e.ChartGraphics.Graphics);
        }
    }
}
