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
using WindowsFormsApp.Custom_Views;
namespace WindowsFormsApp
{
    public partial class MainForm : Form
    {
        Action<string> t;
        FxcmWrapper f = new FxcmWrapper();
        public MainForm()
        {
            InitializeComponent();
            t = new Action<string>(setLabelText);
            string symbol = "USD/CAD";

            //f.SessionStatusChanged += (sender, sessionStatusEnum) =>
            //{
            //    if (label_connectionStatus.InvokeRequired)
            //    {
            //        this.Invoke((Action)(() =>
            //        {
            //            setLabelText(sessionStatusEnum.ToString());

            //        }));

            //    }
            //    else
            //    {
            //        setLabelText(sessionStatusEnum.ToString());
            //    }
            //};


            try
            {
                f.Login("U10D2386411", "1786", "http://www.fxcorporate.com/Hosts.jsp", "Demo");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(0);
            }

        }

        private LegAnalyzer GetHistoricalData(string symbol, Resolution resolution)
        {
            DateTime dailyStartDateTime = new DateTime(2018, 5, 12, 0, 0, 0);
            var dailyEndDateTime = DateTime.Now;//new DateTime(2018, 8, 13, 23, 59, 59); 
            //DateTime dailyEndDateTime = new DateTime(now.Year, now.Month, , 0, 0, 0);

            List<FxBar> dailyBarList = null;
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

            return dailyAnalyzer;
        }

        private void hlocChart3_Load(object sender, EventArgs e)
        {

        }

        private void newChartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dialogBox = new NewChartOptionsPicker())
            {
                var result = dialogBox.ShowDialog();
                if(result == DialogResult.OK)
                {
                    //var selectedTimeFrame = dialogBox.comboBox_timeFrame.SelectedValue;
                    //var s = (TimeFrame)dialogBox.comboBox_timeFrame.SelectedItem;

                    TimeFrame t = (TimeFrame)Enum.Parse(typeof(TimeFrame), dialogBox.comboBox_timeFrame.SelectedItem.ToString());

                    int size = Convert.ToInt16(dialogBox.textBox_timeFrame_size.Text);

                    var la = GetHistoricalData(dialogBox.textBox_symbol.Text, new Resolution(t, size));

                    var chart = new HlocChartForm();

                    chart.LegAnalyzer = la;
                    chart.DrawAnalyzer();
                    chart.Show();
                }
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            f.Logout();
        }

        private void setLabelText(string txt)
        {
            label_connectionStatus.Text = f.SessionStatusEnum.ToString();
            label_connectionStatus.Text = txt;
            label_connectionStatus.BorderStyle = BorderStyle.Fixed3D;
            label_connectionStatus.BackColor = Color.Green;
            label_connectionStatus.ForeColor = Color.White;
        }

    }
}
