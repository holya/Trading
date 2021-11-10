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
//using Trading.DataBases.TextFileDataBase;
using Trading.DataProviders.Common;
using Unity;
using Trading;
using Trading.DataManager.Common;
using System;

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
        
        List<IChartView> chartFormList = new List<IChartView>();

        InstrumentsManager symbolsManager = new InstrumentsManager();

        Label selectedSymbolLabel = null;
        Color selectedSymbolLabelColor = Color.LawnGreen;
        Color normalSymbolLabelColor = Color.FromArgb(192, 192, 255);

        Color sessionButtonBackColor_On = Color.FromArgb(79, 145, 226);
        Color sessionButtonBackColor_Off = Color.FromArgb(226, 79, 84);

        public MainForm(IDataManager dataManager)
        {
            InitializeComponent();

            try
            {
                this.dataManager = dataManager;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Environment.Exit(1);
            }

            this.UpdateSessionStatusButton();


            string isOnline = dataManager.IsOnline ? "Online" : "Offline";
            this.Text = $"Trading App({isOnline})";

            populateSymbols();

            //???????????????????????? 225
            //this.tableLayoutPanel_chartForm.RowStyles.Add(new RowStyle
            //{
            //    SizeType = SizeType.AutoSize
            //});
            //this.tableLayoutPanel_chartForm.RowCount = 2;
            //tableLayoutPanel_chartForm.ColumnStyles.Clear();
            //tableLayoutPanel_chartForm.RowStyles.Clear();
            //tableLayoutPanel_chartForm.ColumnCount = 0;
            //tableLayoutPanel_chartForm.RowCount = 0;
            addNewChartFormToRightPanel(new Resolution(TimeFrame.Monthly, 1), DateTime.UtcNow.AddMonths(-24), DateTime.UtcNow);

            addNewChartFormToRightPanel(new Resolution(TimeFrame.Weekly, 1), DateTime.UtcNow.AddMonths(-6), DateTime.UtcNow);

            addNewChartFormToRightPanel(new Resolution(TimeFrame.Daily, 1), DateTime.UtcNow.AddDays(-30), DateTime.UtcNow);

            addNewChartFormToRightPanel(new Resolution(TimeFrame.Hourly, 4), DateTime.UtcNow.AddDays(-6), DateTime.Now.AddHours(12));

            //addNewChartFormToRightPanel(new Resolution(TimeFrame.Hourly, 1), DateTime.UtcNow.AddHours(-24), DateTime.UtcNow);

            //addNewChartFormToRightPanel(new Resolution(TimeFrame.Minute, 15), DateTime.UtcNow.AddHours(-5), DateTime.UtcNow);

            //addNewChartFormToRightPanel(new Resolution(TimeFrame.Minute, 5), DateTime.UtcNow.AddHours(-2), DateTime.UtcNow);

            //addNewChartFormToRightPanel(new Resolution(TimeFrame.Minute, 1), DateTime.UtcNow.AddMinutes(-20), DateTime.UtcNow);

            //dataManager.SessionStatusChanged += DataManager_SessionStatusChanged;
            dataManager.RealTimeTickUpdated += DataManager_RealTimeTickUpdated;
            dataManager.RealTimeNewBarAdded += DataManager_RealTimeNewBarAdded;

            this.logIn().GetAwaiter();
        }

        #region DataManager events implementations 
        private void DataManager_RealTimeNewBarAdded(object sender, RTNewBarEventArgs e)
        {
            for (int i = 0; i < chartFormList.Count; i++)
            {
                var chart = chartFormList[i];

                if(chart.Instrument.Name == e.Instrument.Name && (chart.Resolution.TimeFrame == e.Resolution.TimeFrame && chart.Resolution.Size == e.Resolution.Size))
                {
                    chart.AddBar(e.Bar);
                }
            }
        }

        private void DataManager_RealTimeTickUpdated(object sender, RTTickUpdateEventArgs e)
        {
            for (int i = 0; i < chartFormList.Count; i++)
            {
                var chart = chartFormList[i];
                if (chart.Instrument.Name == e.Instrument.Name)
                {
                    chart.AddTick(e.Price, e.Volume, e.DateTime);
                }
            }
        }

        private void DataManager_SessionStatusChanged(object sender, SessionStatusChangedEventArgs e)
        {
            if (this.InvokeRequired)
                Invoke(new Action(UpdateSessionStatusButton));
            else
                this.UpdateSessionStatusButton();

        }

        #endregion
        private void UpdateSessionStatusButton()
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

            var sessionMessage = await dataManager.Login("U10D2442130", "7400", "http://www.fxcorporate.com/Hosts.jsp", "Demo");
            //var sessionMessage = await dataManager.Login("holya", "maryam");

            if (sessionMessage.SessionStatus != SessionStatusEnum.Connected)
                MessageBox.Show(sessionMessage.Message, "Problem", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //else
                this.button_sessionStatus.Enabled = true;

            this.UpdateSessionStatusButton();
        }

        private void addNewChartFormToRightPanel(Resolution resolution, DateTime fromDateTime, DateTime toDateTime)
        {
            IChartView chartForm = this.createChartForm();
            //chartForm.Dock = DockStyle.Fill;
            chartForm.Resolution = resolution;
            chartForm.FromDateTime = fromDateTime;
            chartForm.ToDateTime = toDateTime;
            //chartForm.SetTitle();
            //c.FormBorderStyle = FormBorderStyle.None;
            //chart.DoubleClick += chart_MouseDoubleClick;
            //chartForm.FormClosing += (sender, e) =>
            //{
            //    if (e.CloseReason == CloseReason.UserClosing)
            //    {
            //        e.Cancel = true;
            //        return;
            //    }
            //};

            var tpl = tableLayoutPanel_chartForm;

            int count = tpl.Controls.Count;

            int column = count % 2 == 0 ? count / 2 : (count - 1) / 2;
            int row = count % 2;

            if (column > tableLayoutPanel_chartForm.ColumnCount)
            {
                tableLayoutPanel_chartForm.ColumnCount = column;
                //tpl.ColumnStyles.Add(new ColumnStyle { SizeType = SizeType.Percent, Width = 100 / (tpl.ColumnCount+2) });
            }

            if (row > tableLayoutPanel_chartForm.RowCount)
            {
                tableLayoutPanel_chartForm.RowCount = row;               
                //tpl.RowStyles.Add(new RowStyle { SizeType = SizeType.Percent, Height = 100 / (tpl.RowCount+2) });
            }


            tableLayoutPanel_chartForm.Controls.Add((Control)chartForm, column, row);



        }
        private IChartView createChartForm()
        {
            var chartForm = new HlocLACustomChart(); //new NevChartUC(); //new HlocLAForm();
            chartFormList.Add(chartForm);
            return chartForm;
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

        private void unsubscribeCurrentSymbol(string symbol)
        {
            foreach (var item in chartFormList)
            {
                dataManager.UnsubscribeRealTime(item.Instrument, item.Resolution);
            }
        }
        private async void symbolLabel_Click(object sender, EventArgs e)
        {
            if (((Label)sender) == selectedSymbolLabel)
                return;
            if (selectedSymbolLabel != null)
            {
                unsubscribeCurrentSymbol(selectedSymbolLabel.Text);
                selectedSymbolLabel.BackColor = normalSymbolLabelColor;
            }

            selectedSymbolLabel = (Label)sender;
            selectedSymbolLabel.BackColor = selectedSymbolLabelColor;

            string symbol = selectedSymbolLabel.Text;
            Instrument instrument = new Instrument { Name = symbol, Type = InstrumentType.Forex };

            foreach (var c in chartFormList)
            {
                c.Instrument = instrument;
                var barList = await getHistoricalDataAsync(instrument, c.Resolution, c.FromDateTime, c.ToDateTime);
                c.Reset();
                if (barList.Count < 1)
                    break;
                    
                c.AddBarList(barList);

                dataManager.SubscribeRealTime(instrument, c.Resolution);
            }
        }

        private async Task<List<Bar>> getHistoricalDataAsync(Instrument instrument, Resolution resolution, DateTime from, DateTime to)
        {
            try
            {
                var bars =  (await dataManager.GetHistoricalDataAsync(instrument, resolution, from, to)).ToList();
                return bars;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
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

                    IChartView chartForm = createChartForm();
                    chartForm.Resolution = res;
                    chartForm.FromDateTime = fromDate;
                    chartForm.ToDateTime = toDate;
                    //chartForm.SetTitle();
                    //chartForm.Show();
                }
            }
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

        private void splitContainer1_DoubleClick(object sender, EventArgs e)
        {
            if(this.splitContainer1.SplitterDistance <= 10)
            {
                this.splitContainer1.SplitterDistance = 170;
                this.splitContainer1.BackColor = Control.DefaultBackColor;
                ;
                return;
            }    

            while(true)
            {
                this.splitContainer1.SplitterDistance -= 20;
                if (this.splitContainer1.SplitterDistance <= 20)
                {
                    this.splitContainer1.SplitterDistance = 0;
                    this.splitContainer1.BackColor = Color.Black;
                    return;
                }
            }
        }
    }
}
