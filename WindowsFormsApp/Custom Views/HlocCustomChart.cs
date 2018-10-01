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

namespace WindowsFormsApp.Custom_Views
{
    public partial class HlocCustomChart : Chart
    {
        public HlocCustomChart()
        {
            InitializeComponent();

            doCustomization();
        }

        private void doCustomization()
        {
            ChartArea chartArea1 = new ChartArea();
            Series series1 = new Series();

            //
            // chartArea1
            //
            chartArea1.AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
            chartArea1.AxisX.IsStartedFromZero = false;
            chartArea1.AxisX.LabelStyle.Interval = 0D;
            chartArea1.AxisX.LabelStyle.IntervalOffset = 0D;
            chartArea1.AxisX.LabelStyle.IntervalOffsetType = DateTimeIntervalType.Auto;
            chartArea1.AxisX.MajorGrid.Interval = 0D;
            chartArea1.AxisX.MajorGrid.IntervalOffset = 0D;
            chartArea1.AxisY2.IntervalAutoMode = IntervalAutoMode.VariableCount;
            chartArea1.AxisY2.IsStartedFromZero = false;
            chartArea1.AxisY2.LabelStyle.Enabled = false;
            chartArea1.AxisY2.MajorTickMark.Enabled = true;
            chartArea1.BackColor = Color.WhiteSmoke;
            chartArea1.Name = "ChartArea1";
            chartArea1.Position.Auto = false;
            chartArea1.Position.Height = 94F;
            chartArea1.Position.Width = 90F;
            chartArea1.Position.Y = 3F;
            ChartAreas.Add(chartArea1);

            //
            // series1
            //
            series1.BorderWidth = 3;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = SeriesChartType.Stock;
            series1.IsXValueIndexed = true;
            series1.Name = "Series1";
            series1.XValueType = ChartValueType.DateTime;
            series1.YAxisType = AxisType.Secondary;
            series1.YValuesPerPoint = 4;
            Series.Add(series1);

        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }
    }
}
