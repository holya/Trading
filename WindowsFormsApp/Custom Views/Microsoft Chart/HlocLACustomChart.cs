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
    public partial class HlocLACustomChart : HlocCustomChart, IChartView
    {
        public LegAnalyzer LegAnalyzer { get; } = new LegAnalyzer();
        public string Symbol { get; set; }
        public Resolution Resolution { get; set; }
        public DateTime FromDateTime { get; set; }
        public DateTime ToDateTime { get; set; }
        public bool DataPopulated { get; set; } = false;
        public Instrument Instrument { get; set; }

        private Action<double, int, DateTime> addTickDelegate = delegate { };

        public HlocLACustomChart()
        {
            InitializeComponent();
            this.ResizeRedraw = true;
            this.Dock = DockStyle.Fill;
            ChartAreas[0].BackColor = Color.Black;
            this.BackColor = Color.Black;
        }

        public void Reset()
        {
            foreach (var series in Series)
            {
                series.Points.Clear();
            }

            LegAnalyzer.Reset();

            addTickDelegate = delegate { };
        }
        //Maybe rename to Draw
        private void drawBars()
        {
            foreach (var leg in LegAnalyzer.LegList)
            {
                foreach (var bar in leg.BarList)
                {
                    addNewDataPoint(bar);
                }
            }
        }

        private void addNewDataPoint(Bar bar)
        {
            string labelString = isTimeFrameIntraday() ? bar.DateTime.ToShortTimeString() : bar.DateTime.ToLongDateString();
            Color c;
            if (bar.Direction > BarDirection.Balance)
                c = Color.Green;
            else if (bar.Direction < BarDirection.Balance)
                c = Color.DarkRed;
            else
                c = Color.DarkGray;
            DataPoint dp = new DataPoint
            {
                AxisLabel = labelString,
                XValue = bar.DateTime.ToOADate(),
                YValues = new double[] { bar.High, bar.Low, bar.Open, bar.Close },
                Color = c
            };

            Series[0].Points.Add(dp);
        }

        public void AddBarList(IEnumerable<Bar> barList)
        {
            LegAnalyzer.AddBarList(barList);
            drawBars();

            addTickDelegate = addTick;
        }

        public void AddBar(Bar bar)
        {
            if (this.InvokeRequired)
                Invoke(new Action<Bar>(AddBar), bar);
            else
                this.addBar(bar);
        }
        private void addBar(Bar bar)
        {
            LegAnalyzer.AddBar(bar);
            addNewDataPoint(bar);
            Invalidate();
        }

        public void AddTick(double price, int volume, DateTime dateTime)
        {
            addTickDelegate(price, volume, dateTime);
        }
        private void addTick(double price, int volume, DateTime dateTime)
        {
            if (this.InvokeRequired)
                Invoke(new Action<double, int, DateTime>(addTickSafe), price, volume, dateTime);
            else
                this.addTickSafe(price, volume, dateTime);
        }
        
        private void addTickSafe(double price, int volume, DateTime dateTime)
        {            
            LegAnalyzer.UpdateLastBar(price, volume);

            DataPoint dp = Series[0].Points.Last();

            if (dp.YValues[3] != price)
            {
                //dp.YValues[3] = price;
                Series[0].Points.Remove(dp);
                this.addNewDataPoint(LegAnalyzer.LastBar);
                //Series[0].Points.Add(dp);
                this.ChartAreas[0].RecalculateAxesScale();
                Invalidate();
            }
        }

        private bool isTimeFrameIntraday()
        {
            return Resolution.TimeFrame < TimeFrame.Daily;
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
            }
        }

        private void drawClose(Graphics g)
        {
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
            {
                //drawRefLines(e.ChartGraphics.Graphics);
                drawClose(e.ChartGraphics.Graphics);
            }
        }


    }
}
