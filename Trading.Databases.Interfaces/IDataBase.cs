﻿using System;
using System.Collections.Generic;
using Trading.Common;

namespace Trading.DataBases.Interfaces
{
    public interface IDataBase
    {
        IEnumerable<Bar> ReadData(Instrument instrument, Resolution resolution, DateTime fromDate, DateTime toDate);
        void WriteData(Instrument instrument, Resolution resolution, IEnumerable<Bar> barList);
        void PrependData(Instrument instrument, Resolution resolution, IEnumerable<Bar> barList);
        void AppendData(Instrument instrument, Resolution resolution, IEnumerable<Bar> barList);
    }
}
