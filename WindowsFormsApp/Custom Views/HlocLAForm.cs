using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Trading.Analyzers.LegAnalyzer;
using Trading.Common;

namespace WindowsFormsApp.Custom_Views
{
    public partial class HlocLAForm : Form
    {
        public readonly HlocLACustomChart chart = new HlocLACustomChart();

        public LegAnalyzer LegAnalyzer { get; set; }
        public Resolution Resolution { get; set; }
        public string Symbol { get; set; }
        public DateTime FromDateTime { get; set; }


        public HlocLAForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.TopLevel = false;
            chart.Dock = System.Windows.Forms.DockStyle.Fill;
            chart.Location = new System.Drawing.Point(0, 0);
            this.chart.Size = new System.Drawing.Size(911, 450);
            this.Controls.Add(chart);

        }

        public void ResetPropsAndReDraw(LegAnalyzer legAnalyzer, Resolution resolution, string symbol, DateTime fromDateTime)
        {
            LegAnalyzer = legAnalyzer;
            Resolution = resolution;
            Symbol = symbol;
            FromDateTime = fromDateTime;
            chart.ReSetPropsAndReDraw(legAnalyzer, resolution, symbol, fromDateTime);
            this.Text = $"{symbol} - {Resolution.TimeFrame}({Resolution.Size})";
            //chart.DrawAnalyzer();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                return;
            }
            base.OnFormClosing(e);
        }
    }
}
