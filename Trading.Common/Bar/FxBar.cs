using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trading.Common
{
    public class FxBar : Bar
    {
        public double AskOpen { get; set; }
        public double AskHigh { get; set; }
        public double AskLow { get; set; }
        public double AskClose { get; set; }

        public FxBar() :base() { } 

        public FxBar(double open, double askOpen, double high, double askHigh, 
            double low, double askLow, double close, double askClose, double volume, 
            DateTime dateTime, DateTime endDateTime, FxBar previousBar = null) : base(open, high, low, close, volume, dateTime, previousBar)
        {
            this.AskOpen = askOpen;
            this.AskHigh = askHigh;
            this.AskLow = askLow;
            this.AskClose = askClose;
        }


        public void Update(FxBar bar)
        {
            base.Update(bar);

            AskOpen = bar.AskOpen;
            AskHigh = bar.AskHigh;
            AskLow = bar.AskLow;
            AskClose = bar.AskClose;
        }

        public override Bar Factory(IEnumerable<Bar> barList)
        {
            var bar = base.Factory(barList) as FxBar;

            bar.AskOpen = (barList.First() as FxBar).AskOpen;
            bar.AskHigh = (barList.Max() as FxBar).AskHigh;
            bar.AskLow = (barList.Min() as FxBar).AskLow;
            bar.AskClose = (barList.Last() as FxBar).Close;

            return bar;
        }

        public override string ToString()
        {
            return $"{Open},{AskOpen},{High},{AskHigh},{Low},{AskLow},{Close},{AskClose},{Volume},{DateTime}";
        }
    }
}
