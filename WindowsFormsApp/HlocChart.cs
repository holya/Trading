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

        private void chart1_PostPaint(object sender, System.Windows.Forms.DataVisualization.Charting.ChartPaintEventArgs e)
        {
            //populateChart();
        }

        public void DrawAnalyzer()
        {
            if (LegAnalyzer.Resolution.TimeFrame == TimeFrame.Hourly ||
                LegAnalyzer.Resolution.TimeFrame == TimeFrame.Minute)
                chart1.ChartAreas[0].AxisX.LabelStyle.Format = "hh : mm : ss";
            else
                chart1.ChartAreas[0].AxisX.LabelStyle.Format = "YY : MM : DD";
            populateChart();
            populateLines();
        }

        private void populateChart()
        {
            var chartSeries = chart1.Series[0];
            Color barColor = Color.Green;

            foreach (var leg in LegAnalyzer.LegList)
            {
                barColor = leg.Direction == LegDirection.Up ? Color.Green : Color.Red;

                for (int i = 0; i < leg.BarCount; i++)
                {
                    var bar = leg.BarList[i];

                    DataPoint dp = new DataPoint
                    {
                        AxisLabel = bar.DateTime.ToString(),
                        //Label = bar.DateTime.ToString(),
                        XValue = chartSeries.Points.Count,
                        YValues = new double[] { bar.High, bar.Low, bar.Open, bar.Close },
                        Color = barColor
                    };

                    chartSeries.Points.Add(dp);
                }
            }
        }

        private void populateLines()
        {
            var refList = LegAnalyzer.RefList;
            var cs2 = chart1.Series[1];
            foreach (var r in refList)
            {
                var dp = new DataPoint();
                var d = chart1.Series[0].Points.FirstOrDefault(p => p.AxisLabel.Equals(r.DateTime.ToString()));
                dp.XValue = d.XValue;
                dp.YValues = new double[] { r.Price };

                cs2.Points.Add(dp);
            }
        }


    }
}
