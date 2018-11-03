using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;
using Trading.DataProviders.Interfaces;
using Trading.Databases.Interfaces;

namespace Trading.DataManager
{
    public class DataManager
    {
        public Task<IEnumerable<Bar>> GetHistoricalDataAsync(string symbol, Resolution resolution,
            DateTime beginDateTime, DateTime endDateTime, bool subscribeToRealTime = false)
        {
            throw new NotImplementedException();
        }

    }
}
