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
        List<HlocLAForm> chartList = new List<HlocLAForm>();
        SymbolsManager symbolsManager = new SymbolsManager();

        public MainForm()
        {
            InitializeComponent();

            logIn();
            populateSymbols();

            this.tableLayoutPanel_chartForm.RowStyles.Add(new RowStyle
            {
                SizeType = SizeType.AutoSize
            });

            addNewChartFormToRightPanel(new Resolution(TimeFrame.Weekly, 1), new DateTime(2018, 01, 01, 00, 00, 00), 0, 0);
            addNewChartFormToRightPanel(new Resolution(TimeFrame.Daily, 1), new DateTime(2018, 06, 01, 00, 00, 00), 0, 1);
            addNewChartFormToRightPanel(new Resolution(TimeFrame.Hourly, 6), new DateTime(2018, 08, 10, 00, 00, 00), 1, 0);
            addNewChartFormToRightPanel(new Resolution(TimeFrame.Hourly, 1), new DateTime(2018, 08, 19, 00, 00, 00), 1, 1);

        }

        private void addNewChartFormToRightPanel(Resolution resolution, DateTime fromDateTime, int column, int row)
        {
            HlocLAForm c = this.createChartForm();
            c.Resolution = resolution;
            c.FromDateTime = fromDateTime;
            tableLayoutPanel_chartForm.Controls.Add(c, column, row);
            c.Show();
        }

        private void populateSymbols()
        {
            tableLayoutPanel1.RowStyles.RemoveAt(0);

            addSymbolLabelRow(creatSymbolLabel("Majors", Color.White, Color.Red));
            createSymbolRows(symbolsManager.GetForexPairsMajor());
            addSymbolLabelRow(creatSymbolLabel("Minors", Color.White, Color.Red));
            createSymbolRows(symbolsManager.GetForexPairsMinor());
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
                var analyzer = GetHistoricalData(symbol, c.Resolution, c.FromDateTime, DateTime.Now);
                c.ResetPropsAndReDraw(analyzer, c.Resolution, symbol, c.FromDateTime);
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

                    HlocLAForm chart = createChartForm();
                    chart.ResetPropsAndReDraw(la, res, symbol, fromDate);
                    chart.Show();
                }
            }
        }

        private HlocLAForm createChartForm()
        {
            var chart = new HlocLAForm();
            chart.Dock = DockStyle.Fill;
            chartList.Add(chart);
            chart.FormClosing += Chart_FormClosing;
            return chart;
        }

        private void Chart_FormClosing(object sender, FormClosingEventArgs e)
        {
            chartList.Remove((HlocLAForm)sender);
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
