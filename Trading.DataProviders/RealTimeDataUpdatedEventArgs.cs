﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;
namespace Trading.DataProviders.Common
{
    public class RealTimeDataUpdatedEventArgs : EventArgs
    {
        //public Tuple<string, double, double, int, DateTime> Data { get; set; }
        public Instrument Instrument;
        public double Price;
        public int Volume;
        public DateTime DateTime;
    }
}