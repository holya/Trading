using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.Brokers.Fxcm
{
    public class Bar
    {
        public double BidOpen { get; set; }
        public double BidHigh { get; set; }
        public double BidLow { get; set; }
        public double BidClose { get; set; }

        public double AskOpen { get; set; }
        public double AskHigh { get; set; }
        public double AskLow { get; set; }
        public double AskClose { get; set; }

        public int Volume { get; set; }

        public DateTime DateTime { get; set; }
    }
}
