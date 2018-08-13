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
using Trading.Brokers.Fxcm;
using Trading.Common;

namespace WindowsFormsApp
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            FxcmWrapper f = new FxcmWrapper();

            string symbol = "USD/JPY";

            f.SessionStatusChanged += (sender, sessionStatusEnum) =>
            {
                //Console.WriteLine(f.SessionStatusEnum + "");
                Txt_loginStat.Text = f.SessionStatusEnum.ToString();
            };

            try
            {
                f.Login("U10D2386411", "1786", "http://www.fxcorporate.com/Hosts.jsp", "Demo");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(0);
            }

            DateTime dailyStartDateTime = new DateTime(2018, 1, 1, 0, 0, 0);
            var dailyEndDateTime = new DateTime(2018, 7, 31, 23, 59, 59); // DateTime.Now;
            //DateTime dailyEndDateTime = new DateTime(now.Year, now.Month, , 0, 0, 0);
            List<FxBar> dailyBarList = null;
            try
            {
                dailyBarList = (List<FxBar>)f.GetHistoricalData(symbol, new Resolution(TimeFrame.Daily, 1), dailyStartDateTime, dailyEndDateTime);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //Environment.Exit(0);

            }

            var dailyAnalyzer = new LegAnalyzer();
            dailyAnalyzer.AddBarList(dailyBarList);
            populateChart(dailyAnalyzer);

            f.Logout();
        }

        private void populateChart(LegAnalyzer la)
        {
            var chartSeries = chart1.Series[0];
            Color barColor = Color.Green;

            foreach(var leg in la.LegList)
            {
                barColor = leg.Direction == LegDirection.Up ? Color.Green : Color.Red; 
                foreach(var bar in leg.BarList)
                {
                    chartSeries.Points.Add(createBarDataPoint(bar.High, bar.Low, bar.Open, bar.Close, bar.DateTime, barColor));
                }
            }
        }
        private DataPoint createBarDataPoint(double high, double low, double open, double close, DateTime dateTime, Color color)
        {
            var cs = chart1.Series[0];
            DataPoint dp = new DataPoint();
            dp.AxisLabel = dateTime.ToShortTimeString();
            dp.XValue = cs.Points.Count;
            dp.YValues = new double[] { high, low, open, close };
            dp.Color = color;
            return dp;
        }

    }
}
