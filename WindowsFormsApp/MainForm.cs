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
        FxcmWrapper f = new FxcmWrapper();
        public MainForm()
        {
            InitializeComponent();

            string symbol = "AUD/USD";

            f.SessionStatusChanged += (sender, sessionStatusEnum) =>
            {
                //Console.WriteLine(f.SessionStatusEnum + "");
                //Txt_loginStat.Text = f.SessionStatusEnum.ToString();
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

            DateTime dailyStartDateTime = new DateTime(2018, 6, 1, 0, 0, 0);
            var dailyEndDateTime = DateTime.Now;//new DateTime(2018, 8, 13, 23, 59, 59); 
            //DateTime dailyEndDateTime = new DateTime(now.Year, now.Month, , 0, 0, 0);
            List<FxBar> dailyBarList = null;
            Resolution resolution = new Resolution(TimeFrame.Daily, 1);
            try
            {
                dailyBarList = (List<FxBar>)f.GetHistoricalData(symbol, resolution, dailyStartDateTime, dailyEndDateTime);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //Environment.Exit(0);

            }

            var dailyAnalyzer = new LegAnalyzer
            {
                Resolution = resolution
            };
            dailyAnalyzer.AddBarList(dailyBarList);

            hlocChart1.LegAnalyzer = dailyAnalyzer;
            hlocChart1.DrawAnalyzer();

            f.Logout();
        }

    }
}
