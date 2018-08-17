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

namespace WindowsFormsApp.Custom_Views
{
    public partial class NewChartOptionsPicker : Form
    {
        public NewChartOptionsPicker()
        {
            InitializeComponent();

            //var tf = new List<string>();
            //foreach(var t in Enum.GetNames(typeof(TimeFrame)))
            //{
            //    //tf.Add(t);
            //    this.comboBox_timeFrame.Items.Add(t);
            //}

            comboBox_timeFrame.Items.AddRange(Enum.GetNames(typeof(TimeFrame)));
        }

        private void textBox_timeFrame_size_TextChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox_timeFrame_size.Text, out int num))
                textBox_timeFrame_size.Text = "";
        }
    }
}
