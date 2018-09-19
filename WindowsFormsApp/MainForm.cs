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

        Label selectedSymbolLabel = null;
        Color selectedSymbolLabelColor = Color.LawnGreen;
        Color normalSymbolLabelColor = Color.FromArgb(192, 192, 255);

        public MainForm()
        {
            InitializeComponent();

            logIn();

            populateSymbols();

            //???????????????????????? 225
            //this.tableLayoutPanel_chartForm.RowStyles.Add(new RowStyle
            //{
            //    SizeType = SizeType.AutoSize
            //});

            //addNewChartFormToRightPanel(new Resolution(TimeFrame.Weekly, 1), new DateTime(2017, 08, 01, 00, 00, 00), 0, 0);
            //addNewChartFormToRightPanel(new Resolution(TimeFrame.Daily, 1), new DateTime(2018, 09, 01, 00, 00, 00), 0, 1);
            addNewChartFormToRightPanel(new Resolution(TimeFrame.Hourly, 1), DateTime.UtcNow.AddDays(-2), 0, 0);

            addNewChartFormToRightPanel(new Resolution(TimeFrame.Minute, 15), DateTime.UtcNow.AddHours(-10), 0, 1);

            addNewChartFormToRightPanel(new Resolution(TimeFrame.Minute, 5), DateTime.UtcNow.AddHours(-3), 1, 0);
            addNewChartFormToRightPanel(new Resolution(TimeFrame.Minute, 1), DateTime.UtcNow.AddMinutes(-30), 1, 1);

            //addNewChartFormToRightPanel(new Resolution(TimeFrame.Minute, 1), new DateTime(2018, 09, 19, 19, 02, 00), 1, 1);

            f.OffersTableUpdated += OffersTableUpdated;
        }

        private void logIn()
        {
            try
            {
                f.Login("U10D2386411", "1786", "http://www.fxcorporate.com/Hosts.jsp", "Demo");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Environment.Exit(0);
            }
        }

        private void addNewChartFormToRightPanel(Resolution resolution, DateTime fromDateTime, int column, int row)
        {
            HlocLAForm chart = this.createChartForm();
            chart.Resolution = resolution;
            chart.FromDateTime = fromDateTime;
            //c.FormBorderStyle = FormBorderStyle.None;
            //chart.DoubleClick += chart_MouseDoubleClick;
            chart.FormClosing += (sender, e) =>
            {
                if (e.CloseReason == CloseReason.UserClosing)
                {
                    e.Cancel = true;
                    return;
                }
            };
            
            tableLayoutPanel_chartForm.Controls.Add(chart, column, row);
            chart.Show();
        }

        #region Symbols Data Grid
        private void populateSymbols()
        {
            //????????????????????? 225
            //tableLayoutPanel1.RowStyles.RemoveAt(0);

            addSymbolLabelRow(creatSymbolLabel("Majors", Color.White, Color.Red));
            createSymbolRows(symbolsManager.GetForexPairsMajor());
            addSymbolLabelRow(creatSymbolLabel("Minors", Color.White, Color.Red));
            createSymbolRows(symbolsManager.GetForexPairsMinor());
        }

        private void createSymbolRows(IEnumerable<string> sList)
        {
            foreach (var symbol in sList)
            {
                Label l = creatSymbolLabel(symbol, normalSymbolLabelColor, Color.Black);
                l.Click += symbolLabel_Click;
                addSymbolLabelRow(l);
            }
        }

        private void addSymbolLabelRow(Label l)
        {
            //tableLayoutPanel1.RowStyles.Add(new RowStyle
            //{
            //    SizeType = SizeType.AutoSize,
            //    Height = 40
            //});
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

        #endregion

        private void symbolLabel_Click(object sender, EventArgs e)
        {
            if(selectedSymbolLabel != null)
                selectedSymbolLabel.BackColor = normalSymbolLabelColor;
            selectedSymbolLabel = (Label)sender;
            selectedSymbolLabel.BackColor = selectedSymbolLabelColor;

            string symbol = selectedSymbolLabel.Text;

            foreach(var c in chartList)
            {
                c.DataPopulated = false;
                var barList = GetHistoricalData(symbol, c.Resolution, c.FromDateTime, DateTime.Now);
                LegAnalyzer analyzer = createAnalyzer(c.Resolution, barList);
                c.ResetPropsAndReDraw(analyzer, c.Resolution, symbol, c.FromDateTime);
                c.DataPopulated = true;
            }
        }

        private static LegAnalyzer createAnalyzer(Resolution res, List<FxBar> barList)
        {
            foreach (var b in barList)
                b.DateTime = Utilities.NormalizeBarDateTime_FXCM(b.DateTime, res);
            var analyzer = new LegAnalyzer(res);
            analyzer.AddBarList(barList);
            return analyzer;
        }


        private List<FxBar> GetHistoricalData(string symbol, Resolution resolution, DateTime from, DateTime to)
        {
            try
            {
                return (List<FxBar>)f.GetHistoricalData(symbol, resolution, from, to, true);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }

        private void OffersTableUpdated(object sender, Tuple<string, double, double, DateTime, int> e)
        {
            for(int i = 0; i < chartList.Count; i++)
            {
                var c = chartList[i];
                if(c.DataPopulated && c.Symbol == e.Item1)
                {
                    FxBar b = (FxBar)c.LegAnalyzer.LastBar;
                    if(e.Item4 < b.DateTime)
                    {
                        FxBar newBar = new FxBar
                        {
                            Open = e.Item2,
                            AskOpen = e.Item3,
                            High = e.Item2,
                            AskHigh = e.Item3,
                            Low = e.Item2,
                            AskLow = e.Item3,
                            Close = e.Item2,
                            AskClose = e.Item3,
                        };

                        c.LegAnalyzer.UpdateLastBar(newBar);
                    }
                    else
                    {
                        var dt = Utilities.NormalizeBarDateTime_FXCM(e.Item4, c.Resolution);
                        FxBar newBar = new FxBar
                        {
                            Open = e.Item2,
                            AskOpen = e.Item3,
                            High = e.Item2,
                            AskHigh = e.Item3,
                            Low = e.Item2,
                            AskLow = e.Item3,
                            Close = e.Item2,
                            AskClose = e.Item3,
                            DateTime = dt
                        };
                        c.LegAnalyzer.AddBar(newBar);

                    }
                    c.Redraw();
                }
            }
        }


        private void newChartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dialogBox = new NewChartDialog())
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

                    var barList = GetHistoricalData(symbol, res, fromDate, toDate);
                    var la = createAnalyzer(res, barList);

                    HlocLAForm chart = createChartForm();
                    chart.ResetPropsAndReDraw(la, res, symbol, fromDate);
                    chart.Show();
                }
            }
        }

        private HlocLAForm createChartForm()
        {
            var chart = new HlocLAForm();
            //chart.Dock = DockStyle.Fill;
            chartList.Add(chart);
            return chart;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            f.Logout();
        }

    }
}
