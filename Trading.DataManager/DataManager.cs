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
            var data = db.ReadData(instrument, resolution).ToList();
            List<Bar> partialLocalData = new List<Bar>();
            if (data.Count() != 0)
            {
                var localFirstBarDateTime = data.First().DateTime;
                var localLastBarDateTime = data.Last().DateTime;
                int firstDateCompare = DateTime.Compare(localFirstBarDateTime, beginDate);
                int lastDateCompare = DateTime.Compare(localLastBarDateTime, endDate);

                if (firstDateCompare > 0 && lastDateCompare == 0)
                {
                    int leftIndex = data.FindIndex(x => beginDate == x.DateTime);
                    partialLocalData = (List<Bar>)data.Skip(leftIndex - 1);
                    return partialLocalData;
                }
                else if (firstDateCompare > 0 && lastDateCompare < 0)
                {
                    int leftIndex = data.FindIndex(x => beginDate == x.DateTime);
                    int rightIndex = data.FindIndex(y => endDate == y.DateTime);
                    partialLocalData = (List<Bar>)data.Skip(leftIndex - 1).Take((data.Count) - leftIndex - rightIndex - 2);
                }
                else if (firstDateCompare == 0 && lastDateCompare < 0)
                {
                    int rightIndex = data.FindIndex(y => endDate == y.DateTime);
                    partialLocalData = (List<Bar>)data.Take(data.Count - (rightIndex - 1));
                }
                else if (firstDateCompare < 0 && lastDateCompare == 0)
                {

                }
                else if (firstDateCompare < 0 && lastDateCompare > 0)
                {

                }
                else if (firstDateCompare < 0 && lastDateCompare < 0)
                {

                }

            }
            var downloadedData = await dataProvider.GetHistoricalDataAsync(instrument, resolution, beginDate, endDate);
            foreach (var v in downloadedData)
            {
                v.EndDateTime = Utilities.GetEndDateTime(v.DateTime, resolution);
            }
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
