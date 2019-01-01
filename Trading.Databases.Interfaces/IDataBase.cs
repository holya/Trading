using System;
using System.Collections.Generic;
using Trading.Common;

namespace Trading.DataBases.Interfaces
{
    public interface IDataBase
    {
        IEnumerable<Bar> ReadLocalData(Instrument instrument, Resolution resolution, DateTime fromDate, DateTime toDate);
        void WriteLocalData(Instrument instrument, Resolution resolution, IEnumerable<Bar> barList);
        void PrependLocalData(Instrument instrument, Resolution resolution, IEnumerable<Bar> barList);
        void AppendLocalData(Instrument instrument, Resolution resolution, IEnumerable<Bar> barList);
    }
}
