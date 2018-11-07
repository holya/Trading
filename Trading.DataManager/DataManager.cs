using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;
using Trading.Brokers.Fxcm;
using Trading.DataProviders.Interfaces;
using Trading.Databases.TextFileDataBase;

namespace Trading.DataManager
{
    public class DataManager
    {
        TextDataBase db = new TextDataBase();

        FxcmWrapper fxm = new FxcmWrapper();

        private async Task<IEnumerable<Bar>> GetHistoricalDataAsync(Instrument instrument, Resolution resolution, 
            DateTime beginDate, DateTime endDate)
        {
            string symbol = instrument.Name;

            if (db.ReadData(instrument, resolution).Count() != 0)
            {
                var localData = db.ReadData(instrument, resolution);
                return localData;
            }
            else
            {
                var downloadedData = fxm.GetHistoricalDataAsync(instrument, resolution, beginDate, endDate);
                db.WriteData(instrument, resolution, downloadedData.Result);
                return await downloadedData;
            }
        }
    }
}
