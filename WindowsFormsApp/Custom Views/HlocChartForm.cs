using System;
using System.Drawing;
using System.Linq;
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

        public string Symbol { get; set; }
        public Resolution Resolution { get; set; }
        public DateTime FromDateTime { get; set; }
        public LegAnalyzer LegAnalyzer { get; set; }

        public void SetProperties(LegAnalyzer legAnalyzer, string symbol, Resolution resolution, DateTime fromDateTime)
        {
            this.LegAnalyzer = legAnalyzer;
            this.Symbol = symbol;
            this.Resolution = resolution;
            this.FromDateTime = fromDateTime;
        }

        public void DrawAnalyzer()
        {
            this.Text = $"{Symbol} - {Resolution.TimeFrame} {Resolution.Size}";

            drawBars();
        }

        private void drawBarsOriginal()
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
        private void drawBars()
        {
            var chartSeries = chart1.Series[0];
            bool isIntrady = isLaIntraday();
            foreach (var leg in LegAnalyzer.LegList)
            {
                foreach (var bar in leg.BarList)
                {
                    string labelString = isIntrady ? bar.DateTime.ToShortTimeString() : bar.DateTime.ToShortDateString();
                    DataPoint dp = new DataPoint
                    {
                        AxisLabel = labelString,
                        XValue = bar.DateTime.ToOADate(),
                        YValues = new double[] { bar.High, bar.Low, bar.Open, bar.Close },
                        Color = leg.Direction == LegDirection.Up ? Color.Green : Color.Red
                    };
                    chartSeries.Points.Add(dp);
                }
            }
        }

        private void drawRefLinesOriginal(Graphics g)
        {
            foreach (var r in LegAnalyzer.RefList)
            {
                string dts = r.DateTime.ToString();
                var d = chart1.Series[0].Points.FirstOrDefault(p => p.AxisLabel.Equals(dts));

                float x1 = (float)chart1.ChartAreas[0].AxisX.ValueToPixelPosition(d.XValue);
                float x2 = (float)chart1.ChartAreas[0].AxisX.ValueToPixelPosition(chart1.Series[0].Points.Count - 1) + 20;
                var y = (float)chart1.ChartAreas[0].AxisY2.ValueToPixelPosition(r.Price);
                g.DrawLine(new Pen(Color.Black, 1), x1, y, x2, y);

                var font = new Font(FontFamily.GenericSerif,8);
                SolidBrush drawBrush = new SolidBrush(Color.Black);
                g.DrawString("" + r.Price, font, drawBrush, x2 + 7, y-10);
            }
        }
        private void drawRefLines(Graphics g)
        {
            foreach (var r in LegAnalyzer.RefList)
            {
                double dts = r.DateTime.ToOADate();
                var d = chart1.Series[0].Points.FirstOrDefault(p => p.XValue.Equals(dts));

                double pointIndex = -1;
                for (int i = 0; i < chart1.Series[0].Points.Count; i++)
                {
                    if (chart1.Series[0].Points[i].XValue == dts)
                    {
                        pointIndex = i;
                        break;
                    }
                }

                float x1 = (float)chart1.ChartAreas[0].AxisX.ValueToPixelPosition(pointIndex);
                float x2 = (float)chart1.ChartAreas[0].AxisX.ValueToPixelPosition(chart1.Series[0].Points.Count - 1) + 20;
                var y = (float)chart1.ChartAreas[0].AxisY2.ValueToPixelPosition(r.Price);
                g.DrawLine(new Pen(Color.Black, 1), x1, y, x2, y);

                var font = new Font(FontFamily.GenericSerif, 8);
                SolidBrush drawBrush = new SolidBrush(Color.Black);
                g.DrawString("" + r.Price, font, drawBrush, x2 + 7, y - 10);
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

        private void chart1_PostPaint(object sender, ChartPaintEventArgs e)
        {
            if(chart1.Series[0].Points.Count > 0)
                drawRefLines(e.ChartGraphics.Graphics);
        }
    }
}
