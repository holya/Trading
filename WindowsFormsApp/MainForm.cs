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
using Trading.DataProviders.Common;
using Unity;
using Trading;
using Trading.DataManager.Common;

namespace WindowsFormsApp
{
    public partial class MainForm : Form
    {
        //Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        //{
        //    var c = new UnityContainer();
        //    ContainerBootStrapper.RegisterTypes(c);
        //    return c;
        //});
        IDataManager dataManager;
        
        List<HlocLAForm> chartFormList = new List<HlocLAForm>();

        InstrumentsManager symbolsManager = new InstrumentsManager();

        Label selectedSymbolLabel = null;
        Color selectedSymbolLabelColor = Color.LawnGreen;
        Color normalSymbolLabelColor = Color.FromArgb(192, 192, 255);

        Color sessionButtonBackColor_On = Color.FromArgb(79, 145, 226);
        Color sessionButtonBackColor_Off = Color.FromArgb(226, 79, 84);

        public MainForm(IDataManager dataManager)
        {
            InitializeComponent();

            //try
            //{
                this.dataManager = dataManager;
                //dataManager.Login();
            //}
            //catch(Exception e)
            //{
            //    MessageBox.Show(e.Message);
            //    Environment.Exit(1);
            //}

            this.update_sessionStatusButton();


            string isOnline = dataManager.IsOnline ? "Online" : "Offline";
            this.Text = $"Trading App({isOnline})";

            populateSymbols();

            //???????????????????????? 225
            //this.tableLayoutPanel_chartForm.RowStyles.Add(new RowStyle
            //{
            //    SizeType = SizeType.AutoSize
            //});
            this.tableLayoutPanel_chartForm.ColumnCount = 4;

            addNewChartFormToRightPanel(new Resolution(TimeFrame.Monthly, 1), DateTime.UtcNow.AddMonths(-24), DateTime.Now.AddMonths(1), 0, 0);

            addNewChartFormToRightPanel(new Resolution(TimeFrame.Weekly, 1), DateTime.UtcNow.AddMonths(-2), DateTime.Now.AddDays(7), 0, 1);

            addNewChartFormToRightPanel(new Resolution(TimeFrame.Daily, 1), DateTime.UtcNow.AddMonths(-1), DateTime.Now.AddDays(1), 1, 0);

            addNewChartFormToRightPanel(new Resolution(TimeFrame.Hourly, 4), DateTime.UtcNow.AddDays(-6), DateTime.Now.AddHours(12), 1, 1);

            addNewChartFormToRightPanel(new Resolution(TimeFrame.Hourly, 1), DateTime.UtcNow.AddHours(-24), DateTime.Now.AddHours(12), 2, 0);

            addNewChartFormToRightPanel(new Resolution(TimeFrame.Minute, 15), DateTime.UtcNow.AddHours(-5), DateTime.Now.AddHours(12), 2, 1);

            //addNewChartFormToRightPanel(new Resolution(TimeFrame.Minute, 5), DateTime.UtcNow.AddHours(-4), 3, 0);

            //addNewChartFormToRightPanel(new Resolution(TimeFrame.Minute, 1), DateTime.Now.AddDays(-3), DateTime.Now,
            //3, 1);

            dataManager.SessionStatusChanged += DataManager_SessionStatusChanged;
            dataManager.RealTimeDataUpdated += DataManager_RealTimeDataUpdated;
        }

        private void DataManager_SessionStatusChanged(object sender, SessionStatusChangedEventArgs e)
        {
            if (this.InvokeRequired)
                Invoke(new Action(update_sessionStatusButton));
            else
                this.update_sessionStatusButton();

        }
        private void update_sessionStatusButton()
        {
            if (dataManager.IsOnline)
            {
                this.button_sessionStatus.BackColor = Color.FromArgb(79, 145, 226);
                this.button_sessionStatus.Text = "OnLine";
                this.Text = "OnLine";
            }
            else
            {
                this.button_sessionStatus.BackColor = Color.FromArgb(226, 79, 84);
                this.button_sessionStatus.Text = "OffLine";
                this.Text = "OffLine";
            }
        }


        private async Task logIn()
        {
            this.button_sessionStatus.Enabled = false;

            try
            {
                await dataManager.Login();
            }
            catch (SessionStatusException e)
            {
                MessageBox.Show(e.Message);
            }
            this.button_sessionStatus.Enabled = true;
        }

        private void addNewChartFormToRightPanel(Resolution resolution, DateTime fromDateTime, DateTime toDateTime, int column, int row)
        {
            HlocLAForm chartForm = this.createChartForm();
            chartForm.Dock = DockStyle.Fill;
            chartForm.Chart.Resolution = resolution;
            chartForm.Chart.FromDateTime = fromDateTime;
            chartForm.Chart.ToDateTime = toDateTime;
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
            createSymbolRows(symbolsManager.GetForexPairs(ForexTypes.major));
            addSymbolLabelRow(creatSymbolLabel("Minors", Color.White, Color.Red));
            createSymbolRows(symbolsManager.GetForexPairs(ForexTypes.minor));
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
                dataManager.UnsubscribeToRealTime(selectedSymbolLabel.Text);
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
                var barList = await getHistoricalDataAsync(instrument, c.Chart.Resolution, c.Chart.FromDateTime, c.Chart.ToDateTime);
                c.Chart.Reset();
                c.Chart.LegAnalyzer.AddBarList(barList);
                c.Chart.DataPopulated = true;

                dataManager.SubscribeToRealTime(instrument.Name);
            }
        }

        private async Task<List<FxBar>> getHistoricalDataAsync(Instrument instrument, Resolution resolution, DateTime from, DateTime to)
        {
            try
            {
                var bars =  (await dataManager.GetHistoricalDataAsync(instrument, resolution, from, to)).Cast<FxBar>().ToList();
                return bars;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }

        private void DataManager_RealTimeDataUpdated(object sender, RealTimeDataUpdatedEventArgs e)
        {
            var tuple = (Tuple<string, double, double, DateTime, int>) e.Data ;
            for (int i = 0; i < chartFormList.Count; i++)
            {
                var chartForm = chartFormList[i];
                var chart = chartForm.Chart;
                if (chart.DataPopulated && chartForm.Chart.Symbol == tuple.Item1)
                {
                    FxBar b = (FxBar)chart.LegAnalyzer.LastBar;
                    if (tuple.Item4 < b.EndDateTime)
                    {
                        FxBar newBar = new FxBar
                        {
                            Open = tuple.Item2,
                            AskOpen = tuple.Item3,
                            High = tuple.Item2,
                            AskHigh = tuple.Item3,
                            Low = tuple.Item2,
                            AskLow = tuple.Item3,
                            Close = tuple.Item2,
                            AskClose = tuple.Item3,
                        };

                        chart.LegAnalyzer.UpdateLastBar(newBar);
                    }
                    else
                    {
                        var dt = Utilities.NormalizeBarDateTime_FXCM(tuple.Item4, chartForm.Chart.Resolution);

                        FxBar newBar = new FxBar
                        {
                            Open = tuple.Item2,
                            AskOpen = tuple.Item3,
                            High = tuple.Item2,
                            AskHigh = tuple.Item3,
                            Low = tuple.Item2,
                            AskLow = tuple.Item3,
                            Close = tuple.Item2,
                            AskClose = tuple.Item3,
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
            dataManager.Logout();
        }

        private async void button_sessionStatus_Click(object sender, EventArgs e)
        {
            if (dataManager.IsOnline)
                dataManager.Logout();
            else
                await this.logIn();
        }
    }
}
