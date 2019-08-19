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
            DateTime dateTime, DateTime endDateTime) : base(open, high, low, close, volume, dateTime, endDateTime)
        {
            this.AskOpen = askOpen;
            this.AskHigh = askHigh;
            this.AskLow = askLow;
            this.AskClose = askClose;
        }

        public void Update(FxBar bar)
        {
            base.Update(bar);

            AskOpen = ((FxBar)bar).AskOpen;
            AskHigh = ((FxBar)bar).AskHigh;
            AskLow = ((FxBar)bar).AskLow;
            AskClose = ((FxBar)bar).AskClose;
        }

        public override string ToString()
        {
            return $"{Open},{AskOpen},{High},{AskHigh},{Low},{AskLow},{Close},{AskClose},{Volume},{DateTime},{EndDateTime}";
        }
    }
}
