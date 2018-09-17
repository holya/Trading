﻿using System;
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

        public FxBar(double open, double askOpen, double high, double askHigh, double low, double askLow, double close, double askClose, double volume, DateTime dateTime) : base(open, high, low, close, volume, dateTime)
        {
            this.AskOpen = askOpen;
            this.AskHigh = askHigh;
            this.AskLow = askLow;
            this.AskClose = askClose;
        }

        public override void Update(Bar bar)
        {
            base.Update(bar);

            AskOpen = ((FxBar)bar).AskOpen;
            if(((FxBar)bar).AskHigh > AskHigh)
                AskHigh = ((FxBar)bar).AskHigh;
            if(((FxBar)bar).AskLow < AskLow)
                AskLow = ((FxBar)bar).AskLow;

            AskClose = ((FxBar)bar).AskClose;
        }
    }
}
