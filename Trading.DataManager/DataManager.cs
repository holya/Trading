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
using Trading.Analyzers.Common;

namespace Trading.DataManager
{
    public class DataManager : IDisposable
    {
        TextDataBase db;
        FxcmWrapper dataProvider;

        public DataManager()
        {
            dataProvider = new FxcmWrapper();
            db = new TextDataBase();
            logIn();
        }


        public async Task<IEnumerable<Bar>> GetHistoricalDataAsync(Instrument instrument, Resolution resolution, 
            DateTime beginDate, DateTime endDate)
        {
            var k = dataProvider.GetServerTime();
            var data = db.ReadData(instrument, resolution).ToList();
            List<Bar> returnData = new List<Bar>();
            if (data.Count() != 0)
            {
                var firstBarDt = data.First().DateTime;
                var lastBarDt = data.Last().DateTime;

                if(beginDate < firstBarDt)
                {
                    IEnumerable<Bar> collection = await dataProvider.GetHistoricalDataAsync(instrument, resolution, beginDate, firstBarDt);
                    db.PrependData(instrument, resolution, collection);
                    data.InsertRange(0, collection);
                }
                else if(beginDate > firstBarDt)
                {
                    int i = data.FindIndex(p => p.DateTime >= beginDate);
                    data.RemoveRange(0, i);
                }
                
                if(endDate > lastBarDt)
                {
                    if (endDate > dataProvider.GetServerTime())
                        endDate = dataProvider.GetServerTime();
                    var collection = await dataProvider.GetHistoricalDataAsync(instrument, resolution, lastBarDt, endDate);
                    db.AppendData(instrument, resolution, collection);
                    data.AddRange(collection);
                }
                else if(endDate < lastBarDt)
                {
                    int i = data.FindIndex(p => p.DateTime >= endDate);
                    data.RemoveRange(i, data.Count);
                }

                return data;
            }

            var list = await dataProvider.GetHistoricalDataAsync(instrument, resolution, beginDate, endDate);
            db.WriteData(instrument, resolution, list);

            return list;
        }
   
        public void Dispose()
        {
            dataProvider.Logout();
            dataProvider.Dispose();
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
