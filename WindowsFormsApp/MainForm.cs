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
using Trading.Utilities;

namespace WindowsFormsApp
{
    public partial class MainForm : Form
    {
        FxcmWrapper f = new FxcmWrapper();

        List<HlocChartForm> chartList = new List<HlocChartForm>();

        public MainForm()
        {
            InitializeComponent();

            logIn();


            var sManager = new SymbolsManager();
            tableLayoutPanel1.RowStyles.RemoveAt(0);

            addSymbolLabelRow(creatSymbolLabel("Majors", Color.White, Color.Red));
            createSymbolRows(sManager.GetForexPairsMajor());
            addSymbolLabelRow(creatSymbolLabel("Minors", Color.White, Color.Red));
            createSymbolRows(sManager.GetForexPairsMinor());


        }

        private void createSymbolRows(IEnumerable<string> sList)
        {
            foreach (var symbol in sList)
            {
                Label l = creatSymbolLabel(symbol, Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255))))), Color.Black);
                l.Click += symbolLabel_Click;
                addSymbolLabelRow(l);
            }
        }

        private void addSymbolLabelRow(Label l)
        {
            tableLayoutPanel1.RowStyles.Add(new RowStyle
            {
                SizeType = SizeType.AutoSize,
                Height = 40
            });
            tableLayoutPanel1.Controls.Add(l, 0, tableLayoutPanel1.RowCount - 1);
            tableLayoutPanel1.RowCount++;
        }

        private Label creatSymbolLabel(string text, Color backColor, Color forecolor)
        {
            //Label l = new Label();
            //l.TextAlign = ContentAlignment.MiddleCenter;
            //l.BorderStyle = BorderStyle.FixedSingle;
            //l.Font = new Font(FontFamily.GenericMonospace, 10, FontStyle.Bold);
            //l.Height = 40;
            //l.Dock = DockStyle.Fill;
            //l.Text = text;

            return new Label
            {
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle,

                Font = new Font(FontFamily.GenericMonospace, 10, FontStyle.Bold),

                Height = 40,
                Dock = DockStyle.Fill,
                Text = text,
                BackColor = backColor,
                ForeColor = forecolor
            };
        }

        private void symbolLabel_Click(object sender, EventArgs e)
        {
            string symbol = ((Label)sender).Text;
            
            foreach(var c in chartList)
            {
                c.chart1.Series[0].Points.Clear();


                var analyzer = GetHistoricalData(symbol, c.Resolution, c.FromDateTime, DateTime.Now);
                c.SetProperties(analyzer, symbol, c.Resolution, c.FromDateTime);
                c.DrawAnalyzer();
                c.Invalidate();
            }
        }

        private void logIn()
        {
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

        private LegAnalyzer GetHistoricalData(string symbol, Resolution resolution, DateTime from, DateTime to)
        {

            List<FxBar> dailyBarList = null;
            try
            {
                dailyBarList = (List<FxBar>)f.GetHistoricalData(symbol, resolution, from, to);
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


        private void newChartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dialogBox = new NewChartOptionsPicker())
            {
                var result = dialogBox.ShowDialog();
                if(result == DialogResult.OK)
                {
                    string symbol = (string)dialogBox.comboBox_symbols.SelectedItem;

                    TimeFrame timeFrame = (TimeFrame)Enum.Parse(typeof(TimeFrame), dialogBox.comboBox_timeFrame.SelectedItem.ToString());
                    int size = Convert.ToInt16(dialogBox.textBox_timeFrame_size.Text);
                    var res = new Resolution(timeFrame, size);

                    var fromDate = dialogBox.dateTimePicker_from.Value;
                    var toDate = dialogBox.dateTimePicker_to.Value;

                    var la = GetHistoricalData(symbol, res, fromDate, toDate);

                    var chart = new HlocChartForm();
                    chart.Symbol = symbol;
                    chart.Resolution = res;
                    chart.FromDateTime = fromDate;
                    chartList.Add(chart);

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

        private void tableLayoutPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            //for(int i = 0; i < tableLayoutPanel1.Controls.Count)
            //    tableLayoutPanel1.Controls[i].Location.
        }
    }
}
