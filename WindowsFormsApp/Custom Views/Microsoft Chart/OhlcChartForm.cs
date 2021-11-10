using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Trading.Common;
using Trading.DataManager.Common;
using Trading.DataProviders.Common;

namespace WindowsFormsApp.Custom_Views
{
    public partial class OhlcChartForm : Form
    {
        protected IDataManager dataManager;
        protected Resolution resolution;

        private HlocLACustomChart chart = new HlocLACustomChart();
        protected OhlcChartForm()
        {
            InitializeComponent();

            this.TopLevel = false;

            foreach (var item in Enum.GetNames(typeof(TimeFrame)))
                this.timeFrame_ComboBox.Items.Add(item);
        }

        public OhlcChartForm(IDataManager dataManager) :this()
        {
            this.dataManager = dataManager;            
        }
        public OhlcChartForm(IDataManager dataManager, Resolution resolution) :this(dataManager)
        {
            this.resolution = resolution;

            this.timeFrame_ComboBox.SelectedIndex = this.timeFrame_ComboBox.Items.IndexOf(resolution.TimeFrame.ToString());
            this.timeFrameSize_textBox.Text = resolution.Size.ToString();

            
        }
    }
}
