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

        public HlocLAForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.ResizeRedraw = true;
            this.TopLevel = false;
            Chart.Dock = System.Windows.Forms.DockStyle.Fill;
            Chart.Location = new System.Drawing.Point(0, 0);
            this.Chart.Size = new System.Drawing.Size(911, 450);
            this.Controls.Add(Chart);

        }

        public void SetTitle()
        {
            this.Text = $"{Chart.Symbol} - {Chart.Resolution.TimeFrame}({Chart.Resolution.Size})";
        }
    }
}
