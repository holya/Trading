using Nevron.Chart;
using Nevron.GraphicsCore;
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
using WindowsFormsApp;
namespace WindowsFormsApp.Custom_Views
{
    public partial class NevChartUC : UserControl, IChartView
    {
		private NStockSeries m_Stock = new NStockSeries();
		private NChart chart;

		public NevChartUC( )
        {
            InitializeComponent();

			// set a chart title
			NLabel title = nevronChart.Labels.AddHeader("Stick Stock Chart");
			title.TextStyle.FontStyle = new NFontStyle("Times New Roman", 18, FontStyle.Italic);
			title.TextStyle.FillStyle = new NColorFillStyle(Color.Blue);

			// no legend
			nevronChart.Legends.Clear();

			// setup chart
			chart = nevronChart.Charts[0];

			// setup X axis
			NValueTimelineScaleConfigurator scaleX = new NValueTimelineScaleConfigurator();
			scaleX.FirstRow.GridStyle.ShowAtWalls = new ChartWallType[] { ChartWallType.Back };
			scaleX.FirstRow.GridStyle.LineStyle = new NStrokeStyle(1, Color.FromArgb(225, 225, 225));
			scaleX.FirstRow.UseGridStyle = true;
			scaleX.FirstRow.InnerTickStyle.Visible = false;
			scaleX.SecondRow.GridStyle.ShowAtWalls = new ChartWallType[] { ChartWallType.Back };
			scaleX.SecondRow.GridStyle.LineStyle = new NStrokeStyle(1, Color.FromArgb(215, 215, 215));
			scaleX.SecondRow.UseGridStyle = true;
			scaleX.SecondRow.InnerTickStyle.Visible = false;
			scaleX.ThirdRow.GridStyle.ShowAtWalls = new ChartWallType[] { ChartWallType.Back };
			scaleX.ThirdRow.GridStyle.LineStyle = new NStrokeStyle(1, Color.FromArgb(205, 205, 205));
			scaleX.ThirdRow.UseGridStyle = true;
			scaleX.ThirdRow.InnerTickStyle.Visible = false;
			// calendar
			NWeekDayRule wdr = new NWeekDayRule(WeekDayBit.All);
			wdr.Saturday = false;
			wdr.Sunday = false;
			scaleX.Calendar.Rules.Add(wdr);
			scaleX.EnableCalendar = true;
			// set configurator
			chart.Axis(StandardAxis.PrimaryX).ScaleConfigurator = scaleX;

			// setup Y axis
			NLinearScaleConfigurator scaleY = (NLinearScaleConfigurator)chart.Axis(StandardAxis.PrimaryY).ScaleConfigurator;
			scaleY.OuterMajorTickStyle.Length = new NLength(3, NGraphicsUnit.Point);
			scaleY.InnerMajorTickStyle.Visible = false;

			NFillStyle stripFill = new NColorFillStyle(Color.FromArgb(234, 233, 237));
			NScaleStripStyle stripStyle = new NScaleStripStyle(stripFill, null, true, 1, 0, 1, 1);
			stripStyle.ShowAtWalls = new ChartWallType[] { ChartWallType.Back };
			stripStyle.Interlaced = true;
			scaleY.StripStyles.Add(stripStyle);

			// add a stock series
			m_Stock = (NStockSeries)chart.Series.Add(SeriesType.Stock);
			m_Stock.CandleStyle = CandleStyle.Stick;
			m_Stock.DataLabelStyle.Visible = false;
			m_Stock.UpStrokeStyle.Width = new NLength(1, NGraphicsUnit.Point);
			m_Stock.UpStrokeStyle.Color = Color.Black;
			m_Stock.DownStrokeStyle.Width = new NLength(1, NGraphicsUnit.Point);
			m_Stock.DownStrokeStyle.Color = Color.Crimson;
			m_Stock.CandleWidth = new NLength(1.3f, NRelativeUnit.ParentPercentage);
			m_Stock.UseXValues = true;
			m_Stock.InflateMargins = true;

			// apply layout
			ConfigureStandardLayout(chart, title, null);

		}
		internal void ConfigureStandardLayout(NChart chart, NLabel title, NLegend legend)
		{
			nevronChart.Panels.Clear();

			if (title != null)
			{
				nevronChart.Panels.Add(title);

				title.DockMode = PanelDockMode.Top;
				title.Padding = new NMarginsL(5, 8, 5, 4);
			}

			if (legend != null)
			{
				nevronChart.Panels.Add(legend);

				legend.DockMode = PanelDockMode.Right;
				legend.Padding = new NMarginsL(1, 1, 5, 5);
			}

			if (chart != null)
			{
				nevronChart.Panels.Add(chart);

				float topPad = (title == null) ? 11 : 8;
				float rightPad = (legend == null) ? 11 : 4;

				if (chart.BoundsMode == BoundsMode.None)
				{
					if (chart.Enable3D || !(chart is NCartesianChart))
					{
						chart.BoundsMode = BoundsMode.Fit;
					}
					else
					{
						chart.BoundsMode = BoundsMode.Stretch;
					}
				}

				chart.DockMode = PanelDockMode.Fill;
				chart.Padding = new NMarginsL(
					new NLength(11, NRelativeUnit.ParentPercentage),
					new NLength(topPad, NRelativeUnit.ParentPercentage),
					new NLength(rightPad, NRelativeUnit.ParentPercentage),
					new NLength(11, NRelativeUnit.ParentPercentage));
			}
		}


		public Instrument Instrument { get; set; }
        public TimeFrame TimeFrame { get; set; }
        public Resolution Resolution { get; set; }
        public DateTime FromDateTime { get; set; }
        public DateTime ToDateTime { get; set; }

        public void AddBar(Bar bar)
        {
            throw new NotImplementedException();
        }

        public void AddBarList(IEnumerable<Bar> bars)
        {
            foreach (var bar in bars)
            {
				m_Stock.OpenValues.Add(bar.Open);
				m_Stock.HighValues.Add(bar.Open);
				m_Stock.LowValues.Add(bar.Open);
				m_Stock.CloseValues.Add(bar.Open);

				m_Stock.XValues.Add(bar.DateTime);
			}
			chart.Refresh();
		}

        public void AddTick(double price, int volume, DateTime dateTime)
        {

        }

        public void Reset( )
        {
			m_Stock.ClearDataPoints();
        }
    }
}
