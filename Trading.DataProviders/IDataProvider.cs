using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;

namespace Trading.DataProviders.Interfaces
{
    public interface IDataProvider
    {
        Task<IEnumerable<Bar>> GetHistoricalDataAsync(string symbol, Resolution resolution,
            DateTime beginDateTime, DateTime endDateTime, bool subscribeToRealTime = false);
    }
}
