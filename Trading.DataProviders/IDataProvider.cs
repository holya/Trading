using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;

namespace Trading.DataProviders
{
    public interface IDataProvider
    {
        List<Bar> GetHistoricalData(string symbol, Resolution resolution,
            DateTime beginDateTime, DateTime endDateTime);
    }
}
