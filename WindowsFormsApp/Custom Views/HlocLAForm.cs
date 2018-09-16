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
        public HlocLACustomChart Chart { get; } = new HlocLACustomChart();

        public LegAnalyzer LegAnalyzer { get; set; }
        public Resolution Resolution { get; set; }
        public string Symbol { get; set; }
        public DateTime FromDateTime { get; set; }
        public bool DataPopulated { get; set; } = false;


        public HlocLAForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.TopLevel = false;
            Chart.Dock = System.Windows.Forms.DockStyle.Fill;
            Chart.Location = new System.Drawing.Point(0, 0);
            this.Chart.Size = new System.Drawing.Size(911, 450);
            this.Controls.Add(Chart);
        }

        public void ResetPropsAndReDraw(LegAnalyzer legAnalyzer, Resolution resolution, string symbol, DateTime fromDateTime)
        {
            LegAnalyzer = legAnalyzer;
            Resolution = resolution;
            Symbol = symbol;
            FromDateTime = fromDateTime;
            Chart.ReSetPropsAndReDraw(legAnalyzer, resolution, symbol, fromDateTime);
            this.Text = $"{symbol} - {Resolution.TimeFrame}({Resolution.Size})";
            //chart.DrawAnalyzer();
        }
        public void Redraw()
        {
            if(InvokeRequired)
                Invoke(new Action(() => Chart.ReSetPropsAndReDraw()));
        }
    }
}
