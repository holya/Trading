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

        public async Task<IEnumerable<Bar>> GetHistoricalDataAsync(Instrument instrument, Resolution resolution, 
            DateTime beginDate, DateTime endDate)
        {
            var data = db.ReadData(instrument, resolution);
            //if beginDate and endDate check ---- convert IEnumerable<Bar> to List<Bar> to add DateTime at the end
            if (data.Count() != 0)         
                return data;
            else
            {
                var downloadedData = await fxm.GetHistoricalDataAsync(instrument, resolution, beginDate, endDate);
                db.WriteData(instrument, resolution, downloadedData);
                return downloadedData;
            }
        }
    }
}
