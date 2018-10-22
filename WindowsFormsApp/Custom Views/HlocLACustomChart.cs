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
        public LegAnalyzer LegAnalyzer { get; } = new LegAnalyzer();
        public string Symbol { get; set; }
        public Resolution Resolution { get; set; }
        public DateTime FromDateTime { get; set; }
        public bool DataPopulated { get; set; } = false;
        public HlocLACustomChart()
        {
            InitializeComponent();
            this.ResizeRedraw = true;
            
            LegAnalyzer.AnalyzerPopulated += LegAnalyzer_AnalyzerPopulated;
            LegAnalyzer.NewBarAdded += LegAnalyzer_NewBarAdded;
            LegAnalyzer.LastBarUpdated += LegAnalyzer_LastBarUpdated;

            ChartAreas[0].BackColor = Color.Black;
            this.BackColor = Color.Black;
        }

        private void LegAnalyzer_LastBarUpdated(object sender, LastBarUpdatedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                Invoke(new Action<LastBarUpdatedEventArgs>(this.updateLastBar), e);
            }
            else
                this.updateLastBar(e);
        }

        private void updateLastBar(LastBarUpdatedEventArgs e)
        {
            DataPoint dp = Series[0].Points.Last();
            switch (e.UpdateEnum)
            {
                case LastbarUpdateEventEnum.NoPriceChange:
                    return;
                case LastbarUpdateEventEnum.CloseUpdated:
                    dp.YValues[3] = e.LastBar.Close;
                    break;
                case LastbarUpdateEventEnum.Expanded:
                case LastbarUpdateEventEnum.TypeChanged:
                    Series[0].Points.Remove(dp);
                    this.addNewDataPoint(Series[0], this.LegAnalyzer.LastLeg, e.LastBar);
                    this.ChartAreas[0].RecalculateAxesScale();
                    break;
                default:
                    break;
            }
            Invalidate();
        }

        private void LegAnalyzer_NewBarAdded(object sender, NewBarAddedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<NewBarAddedEventArgs>(addNewbar), e);
            }
        }

        private void addNewbar(NewBarAddedEventArgs e)
        {
            addNewDataPoint(Series[0], e.LastLeg, e.LastLeg.LastBar);
            Invalidate();
        }

        private void LegAnalyzer_AnalyzerPopulated(object sender, AnalyzerPopulatedEventArgs e)
        {
            setupBars();
            //Invalidate();
        }

        public void Reset()
        {
            foreach (var series in Series)
            {
                if (series.Points.Count > 0)
                    series.Points.Clear();
            }
            LegAnalyzer.Reset();
        }
        //Maybe rename to Draw
        private void setupBars()
        {
            foreach (var leg in LegAnalyzer.LegList)
            {
                foreach (var bar in leg.BarList)
                {
                    addNewDataPoint(Series[0], leg, bar);
                }
            }
        }

        private void addNewDataPoint(Series chartSeries, Leg leg, Bar bar)
        {
            string labelString = isTimeFrameIntraday() ? bar.DateTime.ToShortTimeString() : bar.DateTime.ToLongDateString();
            DataPoint dp = new DataPoint
            {
                AxisLabel = labelString,
                XValue = bar.DateTime.ToOADate(),
                YValues = new double[] { bar.High, bar.Low, bar.Open, bar.Close },
                Color = leg.Direction == LegDirection.Up ? Color.Green : Color.DarkRed
            };
            chartSeries.Points.Add(dp);
            //chartSeries.Points.Last().YValues[3] = ;
        }

        private bool isTimeFrameIntraday()
        {
            return Resolution.TimeFrame < TimeFrame.Daily;
        }

        private void drawRefLines(Graphics g)
        {
            //foreach (var r in LegAnalyzer.RefList)
            //{
            //    double dts = r.DateTime.ToOADate();
            //    var d = Series[0].Points.FirstOrDefault(p => p.XValue.Equals(dts));

            //    double pointIndex = -1;
            //    for (int i = 0; i < Series[0].Points.Count; i++)
            //    {
            //        if (Series[0].Points[i].XValue == dts)
            //        {
            //            pointIndex = i;
            //            break;
            //        }
            //    }

            //    float x1 = (float)ChartAreas[0].AxisX.ValueToPixelPosition(pointIndex + 1);
            //    float x2 = (float)ChartAreas[0].AxisX.ValueToPixelPosition(Series[0].Points.Count - 1) + 20;
            //    var y = (float)ChartAreas[0].AxisY2.ValueToPixelPosition(r.Price);
            //    g.DrawLine(new Pen(Color.Black, 1), x1, y, x2, y);

            //    var font = new Font(FontFamily.GenericSerif, 8);
            //    SolidBrush drawBrush = new SolidBrush(Color.Black);
            //    g.DrawString("" + r.Price, font, drawBrush, x2 + 7, y - 10);
            //}

            //draw close value
            float diff = (float)ChartAreas[0].AxisX.ValueToPixelPosition(Series[0].Points.Count - 1) - Right;

            float xCoord = Right - 60;
            var yCoord = (float)ChartAreas[0].AxisY2.ValueToPixelPosition(LegAnalyzer.Close);
            var f = new Font(FontFamily.GenericSerif, 8);
            SolidBrush db = new SolidBrush(Color.White);
            g.DrawString("" + LegAnalyzer.Close, f, db, xCoord, yCoord - 10);
            
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
