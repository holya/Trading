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
using Trading.DataManager;
using Trading.DataBases.TextFileDataBase;

namespace WindowsFormsApp
{
    public partial class MainForm : Form
    {
        FxcmWrapper f = new FxcmWrapper();

        //TextDataBase dataBase = new TextDataBase();
        DataManager dataManager = new DataManager();
        
        List<HlocLAForm> chartFormList = new List<HlocLAForm>();

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


            addNewChartFormToRightPanel(new Resolution(TimeFrame.Monthly, 1), DateTime.UtcNow.AddMonths(-24), 0, 0);

            addNewChartFormToRightPanel(new Resolution(TimeFrame.Weekly, 1), DateTime.UtcNow.AddMonths(-2), 0, 1);

            addNewChartFormToRightPanel(new Resolution(TimeFrame.Daily, 1), DateTime.UtcNow.AddMonths(-1), 1, 0);

            addNewChartFormToRightPanel(new Resolution(TimeFrame.Hourly, 4), DateTime.UtcNow.AddDays(-6), 1, 1);

            addNewChartFormToRightPanel(new Resolution(TimeFrame.Hourly, 1), DateTime.UtcNow.AddHours(-24), 2, 0);

            addNewChartFormToRightPanel(new Resolution(TimeFrame.Minute, 15), DateTime.UtcNow.AddHours(-5), 2, 1);




            //addNewChartFormToRightPanel(new Resolution(TimeFrame.Minute, 5), DateTime.UtcNow.AddHours(-4), 2, 1);

            //addNewChartFormToRightPanel(new Resolution(TimeFrame.Minute, 1), DateTime.UtcNow.AddMinutes(-10), 1, 1);

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
            HlocLAForm chartForm = this.createChartForm();
            chartForm.Dock = DockStyle.Fill;
            chartForm.Chart.Resolution = resolution;
            chartForm.Chart.FromDateTime = fromDateTime;
            chartForm.SetTitle();
            //c.FormBorderStyle = FormBorderStyle.None;
            //chart.DoubleClick += chart_MouseDoubleClick;
            chartForm.FormClosing += (sender, e) =>
            {
                if (e.CloseReason == CloseReason.UserClosing)
                {
                    e.Cancel = true;
                    return;
                }
            };
            
            tableLayoutPanel_chartForm.Controls.Add(chartForm, column, row);
            chartForm.Show();
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

        private async void symbolLabel_Click(object sender, EventArgs e)
        {
            if (((Label)sender) == selectedSymbolLabel)
                return;
            if (selectedSymbolLabel != null)
            {
                f.UnsubscribeToRealTime(selectedSymbolLabel.Text);
                selectedSymbolLabel.BackColor = normalSymbolLabelColor;
            }

            selectedSymbolLabel = (Label)sender;
            selectedSymbolLabel.BackColor = selectedSymbolLabelColor;

            string symbol = selectedSymbolLabel.Text;
            Instrument instrument = new Instrument { Name = symbol, Type = InstrumentType.Forex };

            foreach (var c in chartFormList)
            {
                c.Chart.DataPopulated = false;
                c.Chart.Symbol = symbol;
                var barList = await getHistoricalDataAsync(instrument, c.Chart.Resolution, c.Chart.FromDateTime, DateTime.UtcNow.AddDays(10));
                c.Chart.Reset();
                c.Chart.LegAnalyzer.AddBarList(barList);
                c.Chart.DataPopulated = true;

                f.SubscribeToRealTime(instrument.Name);
            }
        }

        private async Task<List<FxBar>> getHistoricalDataAsync(Instrument instrument, Resolution resolution, DateTime from, DateTime to)
        {
            try
            {
                var bars = await f.GetHistoricalDataAsync(instrument, resolution, from, to);
                return (List<FxBar>)bars;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }


        private void OffersTableUpdated(object sender, Tuple<string, double, double, DateTime, int> e)
        {
            for(int i = 0; i < chartFormList.Count; i++)
            {
                var chartForm = chartFormList[i];
                var chart = chartForm.Chart;
                if(chart.DataPopulated && chartForm.Chart.Symbol == e.Item1)
                {
                    FxBar b = (FxBar)chart.LegAnalyzer.LastBar;
                    if(e.Item4 < b.EndDateTime)
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

                        chart.LegAnalyzer.UpdateLastBar(newBar);
                    }
                    else
                    {
                        var dt = Utilities.NormalizeBarDateTime_FXCM(e.Item4, chartForm.Chart.Resolution);

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
                            DateTime = dt,
                            EndDateTime = Utilities.GetEndDateTime(dt, chart.Resolution)
                        };
                        chart.LegAnalyzer.AddBar(newBar);
                    }
                }
            }
        }


        private async void newChartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dialogBox = new NewChartDialog())
            {
                var result = dialogBox.ShowDialog();
                if(result == DialogResult.OK)
                {
                    string symbol = (string)dialogBox.comboBox_symbols.SelectedItem;
                    Instrument instrument = new Instrument { Name = symbol, Type = InstrumentType.Forex };
                    TimeFrame timeFrame = (TimeFrame)Enum.Parse(typeof(TimeFrame), dialogBox.comboBox_timeFrame.SelectedItem.ToString());
                    int size = Convert.ToInt16(dialogBox.textBox_timeFrame_size.Text);
                    var res = new Resolution(timeFrame, size);

                    var fromDate = dialogBox.dateTimePicker_from.Value;
                    var toDate = dialogBox.dateTimePicker_to.Value;

                    var barList = await getHistoricalDataAsync(instrument, res, fromDate, toDate);

                    HlocLAForm chartForm = createChartForm();
                    chartForm.Chart.Resolution = res;
                    chartForm.Chart.FromDateTime = fromDate;
                    chartForm.SetTitle();
                    chartForm.Show();
                }
            }
        }

        private HlocLAForm createChartForm()
        {
            var chartForm = new HlocLAForm();
            chartFormList.Add(chartForm);
            return chartForm;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            f.Logout();
        }

    }
}
