﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.DataProviders.Common
{
    public class RealTimeDataUpdatedEventArgs : EventArgs
    {
        public Tuple<string, double, double, DateTime, int> Data { get; set; }
    }
}
