using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;
using Trading.DataProviders.Interfaces;
using Trading.Databases.Interfaces;
using Trading.Common.Instrument;

namespace Trading.DataManager
{
    public class DataManager : IDataBase, IDataProvider
    {
        public IEnumerable<Bar> ReadData(Instrument instrument, Resolution resolution)
        {
            throw new NotImplementedException();
        }

        public void WriteData(Instrument instrument, Resolution resolution, IEnumerable<Bar> barList)
        {

        }

        public Task<IEnumerable<Bar>> GetHistoricalDataAsync(string symbol, Resolution resolution,
            DateTime beginDateTime, DateTime endDateTime, bool subscribeToRealTime = false)
        {
            throw new NotImplementedException();
        }

    }
}
