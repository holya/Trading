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
using Trading.Utilities;

namespace WindowsFormsApp.Custom_Views
{
    public partial class NewChartDialog : Form
    {
        public NewChartDialog()
        {
            InitializeComponent();
            
            comboBox_timeFrame.Items.AddRange(Enum.GetNames(typeof(TimeFrame)));
            comboBox_timeFrame.SelectedIndex = 5;

            var sm = new SymbolsManager();
            comboBox_symbols.Items.Add("Major Pairs");
            comboBox_symbols.Items.AddRange(sm.GetForexPairsMajor().ToArray());
            comboBox_symbols.Items.Add("Minor Pairs");
            comboBox_symbols.Items.AddRange(sm.GetForexPairsMinor().ToArray());
            comboBox_symbols.SelectedIndex = 1;

            dateTimePicker_from.Value = dateTimePicker_to.Value.Date.AddDays(-4);
        }

        private void textBox_timeFrame_size_TextChanged(object sender, EventArgs e)
        {
            int num;
            if (!int.TryParse(textBox_timeFrame_size.Text, out num))
                textBox_timeFrame_size.Text = "";
        }

        private void button_ok_Click(object sender, EventArgs e)
        {

        }

        private void comboBox_symbols_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
                return;
            Font myFont = new Font("Aerial", 8, FontStyle.Regular);
            Font myFont2 = new Font("Aerial", 10, FontStyle.Regular | FontStyle.Underline);
            
            if (e.Index == 0 || e.Index == 8)
                e.Graphics.DrawString(comboBox_symbols.Items[e.Index].ToString(), myFont2, Brushes.DarkRed, e.Bounds);
            else
            {
                e.DrawBackground();
                e.Graphics.DrawString(comboBox_symbols.Items[e.Index].ToString(), myFont, Brushes.Black, e.Bounds);
                e.DrawFocusRectangle();
            }
        }

        private void comboBox_symbols_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_symbols.SelectedIndex == 0 || comboBox_symbols.SelectedIndex == 8)
                comboBox_symbols.SelectedIndex = -1;
        }
    }
}
