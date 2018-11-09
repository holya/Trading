using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;
using Trading.Brokers.Fxcm;
using Trading.Databases.Interfaces;
using Trading.DataProviders.Interfaces;
using Trading.Databases.TextFileDataBase;

namespace Trading.DataManager
{
    public class DataManager : IDisposable
    {
        TextDataBase db;
        public FxcmWrapper dataProvider;

        public DataManager()
        {
            dataProvider = new FxcmWrapper();
            db = new TextDataBase();
            logIn();
        }


        public async Task<IEnumerable<Bar>> GetHistoricalDataAsync(Instrument instrument, Resolution resolution, 
            DateTime beginDate, DateTime endDate)
        {
            var data = db.ReadData(instrument, resolution);
            while (data.Count() != 0)
            {
                var firstLocalBarDateTime = data.First().DateTime;
                int comparison = DateTime.Compare(firstLocalBarDateTime, beginDate);
                if (comparison > 0)
                    break;
                return data;
            }
            var downloadedData = await dataProvider.GetHistoricalDataAsync(instrument, resolution, beginDate, endDate);
            db.WriteData(instrument, resolution, downloadedData);
            return downloadedData;
        }

        public void Dispose()
        {
            dataProvider.Logout();
        }

        private void logIn()
        {
            try
            {
                dataProvider.Login("U10D2386411", "1786", "http://www.fxcorporate.com/Hosts.jsp", "Demo");
            }
            catch (Exception e)
            {
                Environment.Exit(0);
            }
        }

    }
}
