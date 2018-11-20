using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;
using Trading.Brokers.Fxcm;
using Trading.DataBases.Interfaces;
using Trading.DataProviders.Interfaces;
using Trading.DataBases.TextFileDataBase;
using Trading.Analyzers.Common;
using System.Xml.Linq;
using Trading.DataBases.XmlDataBase;

namespace Trading.DataManager
{
    public class DataManager : IDisposable
    {
        IDataBase repository;
        FxcmWrapper dataProvider;

        public DataManager()
        {
            dataProvider = new FxcmWrapper();
            repository = new XmlDataBase();
            logIn();
        }

        public async Task<IEnumerable<Bar>> GetHistoricalDataAsync(Instrument instrument, Resolution resolution, 
            DateTime beginDate, DateTime endDate)
        {
            var localData = repository.ReadData(instrument, resolution).ToList();

            if (localData.Count() != 0)
            {
                var firstLocalBarDateTime = localData.First().DateTime;
                var lastLocalBarDateTime = localData.Last().DateTime;

                if(beginDate < firstLocalBarDateTime)
                {
                    IEnumerable<Bar> prependBarList = await dataProvider.GetHistoricalDataAsync(instrument, resolution, beginDate, firstLocalBarDateTime);
                    repository.PrependData(instrument, resolution, prependBarList);
                    localData.InsertRange(0, prependBarList);
                }
                else if(beginDate > firstLocalBarDateTime)
                {
                    int i = localData.FindIndex(p => p.DateTime >= beginDate);
                    localData.RemoveRange(0, i);
                }
                
                if(endDate > lastLocalBarDateTime)
                {
                    var appendBarList = await dataProvider.GetHistoricalDataAsync(instrument, resolution, lastLocalBarDateTime, endDate);
                    repository.AppendData(instrument, resolution, appendBarList);
                    if (appendBarList.First().DateTime == localData.Last().DateTime)
                        localData.Remove(localData.Last());
                    localData.AddRange(appendBarList);
                }
                else if(endDate < lastLocalBarDateTime)
                {
                    int removeBarIndex = localData.FindIndex(p => p.DateTime >= endDate);
                    localData.RemoveRange(removeBarIndex, localData.Count - removeBarIndex);
                }

                return localData;
            }

            var list = await dataProvider.GetHistoricalDataAsync(instrument, resolution, beginDate, endDate);
            repository.WriteData(instrument, resolution, list);

            return list;
        }

        public void SubscribeToRealTime(string instrument)
        {
            dataProvider.SubscribeToRealTime(instrument);
        }

        public void UnsubscribeToRealTime(string instrument)
        {
            dataProvider.UnsubscribeToRealTime(instrument);
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
