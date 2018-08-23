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
    public partial class HlocLACustomChart : HlocCustomChart
    {
        public LegAnalyzer LegAnalyzer { get; set; }
        public Resolution Resolution { get; set; }
        public string Symbol { get; set; }
        public DateTime FromDateTime { get; set; }

        public HlocLACustomChart()
        {
            InitializeComponent();
        }
        public HlocLACustomChart(LegAnalyzer legAnalyzer, Resolution resolution, string symbol, DateTime fromDateTime) :this()
        {
            ReSetPropsAndReDraw(legAnalyzer, resolution, symbol, fromDateTime);
        }

        public void ReSetPropsAndReDraw(LegAnalyzer legAnalyzer, Resolution resolution, string symbol, DateTime fromDateTime)
        {
            foreach(var series in Series)
            {
                if (series.Points.Count > 0)
                    series.Points.Clear();
            }

            LegAnalyzer = legAnalyzer;
            Resolution = resolution;
            Symbol = symbol;
            FromDateTime = fromDateTime;

            setupBars();
            Invalidate();
        }

        public void DrawAnalyzer()
        {
            setupBars();
        }

        private void setupBars()
        {
            var chartSeries = Series[0];
            bool isIntrady = isLAIntraday();
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

        private void drawRefLines(Graphics g)
        {
            foreach (var r in LegAnalyzer.RefList)
            {
                double dts = r.DateTime.ToOADate();
                var d = Series[0].Points.FirstOrDefault(p => p.XValue.Equals(dts));

                double pointIndex = -1;
                for (int i = 0; i < Series[0].Points.Count; i++)
                {
                    if (Series[0].Points[i].XValue == dts)
                    {
                        pointIndex = i;
                        break;
                    }
                }

                float x1 = (float)ChartAreas[0].AxisX.ValueToPixelPosition(pointIndex + 1);
                float x2 = (float)ChartAreas[0].AxisX.ValueToPixelPosition(Series[0].Points.Count - 1) + 20;
                var y = (float)ChartAreas[0].AxisY2.ValueToPixelPosition(r.Price);
                g.DrawLine(new Pen(Color.Black, 1), x1, y, x2, y);

                var font = new Font(FontFamily.GenericSerif, 8);
                SolidBrush drawBrush = new SolidBrush(Color.Black);
                g.DrawString("" + r.Price, font, drawBrush, x2 + 7, y - 10);
            }
        }
        
        private bool isLAIntraday()
        {
            return (Resolution.TimeFrame == TimeFrame.Hourly ||
                Resolution.TimeFrame == TimeFrame.Minute) ? true : false;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        protected override void OnPostPaint(ChartPaintEventArgs e)
        {
            base.OnPostPaint(e);

            if (Series[0].Points.Count > 0)
                drawRefLines(e.ChartGraphics.Graphics);
        }
    }
}
