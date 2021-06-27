using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trading.Common;

namespace Trading.DataBases.Common
{
    public interface IDataBase
    {
        Task<IEnumerable<Bar>> ReadLocalDataAsync(Instrument instrument, Resolution resolution, DateTime fromDate, DateTime toDate);
        Task<IEnumerable<Bar>> ReadLocalDataAsync(Instrument instrument, Resolution resolution);

        Task<bool> WriteLocalDataAsync(Instrument instrument, Resolution resolution, IEnumerable<Bar> barList);

        Task<bool> PrependLocalData(Instrument instrument, Resolution resolution, IEnumerable<Bar> barList);

        Task<bool> AppendLocalData(Instrument instrument, Resolution resolution, IEnumerable<Bar> barList);
        bool FileExists(Instrument instrument, Resolution resolution);
    }
}
