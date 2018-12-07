using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;
using Trading.Brokers.Fxcm;
using Trading.DataBases.Interfaces;
using Trading.DataProviders.Common;
using Trading.DataBases.TextFileDataBase;
using Trading.Analyzers.Common;
using System.Xml.Linq;
using Trading.DataBases.XmlDataBase;

namespace Trading.DataManager
{
    public class DataManager : IDisposable
    {
        public event EventHandler<DataUpdatedEventArgs> DataUpdated;

        IDataBase repository;
        IDataProvider dataProvider;

        public bool IsOnline { get; private set; } = false;

        public DataManager()
        {
            repository = new XmlDataBase();
            dataProvider = new FxcmWrapper();
            logIn();
            dataProvider.DataUpdated += DataProvider_DataUpdated;
        }

        public DataManager(IDataProvider dataProvider, IDataBase dataBase)
        {
            this.dataProvider = dataProvider;
            repository = dataBase;
            logIn();
            dataProvider.DataUpdated += DataProvider_DataUpdated;
        }

        private void DataProvider_DataUpdated(object sender, DataUpdatedEventArgs e)
        {
            DataUpdated?.Invoke(sender, e);
        }
        
        public async Task<IEnumerable<Bar>> GetHistoricalDataAsync(Instrument instrument, Resolution resolution, 
            DateTime beginDate, DateTime endDate)
        {
            var localData = repository.ReadData(instrument, resolution, beginDate, endDate).ToList();

            if (localData.Count() != 0)
            {
                var firstLocalBarDateTime = localData.First().DateTime;
                var lastLocalBarDateTime = localData.Last().DateTime;

                if(beginDate < firstLocalBarDateTime)
                {
                    var prependBarList = (await dataProvider.GetHistoricalDataAsync(instrument, resolution, beginDate, firstLocalBarDateTime)).ToList();

                    if (prependBarList.Count > 0 && prependBarList.Last().DateTime == firstLocalBarDateTime)
                        prependBarList.Remove(prependBarList.Last());
                    if (prependBarList.Count() > 0)
                    {
                        repository.PrependData(instrument, resolution, prependBarList);
                        localData.InsertRange(0, prependBarList);
                    }
                }
                else if(beginDate > firstLocalBarDateTime)
                {
                    if(beginDate >= lastLocalBarDateTime)
                    {
                        var list = (await dataProvider.GetHistoricalDataAsync(instrument, resolution, lastLocalBarDateTime, endDate)).ToList();
                        repository.AppendData(instrument, resolution, list);

                        int removeIndex = list.FindIndex(bar => bar.DateTime >= beginDate & bar.EndDateTime <= beginDate);
                        if(removeIndex > 0)
                        {
                            list.RemoveRange(0, removeIndex);
                        }

                        return list;
                    }
                    int i = localData.FindIndex(p => p.DateTime <= beginDate && p.EndDateTime >= beginDate);
                    localData.RemoveRange(0, i);
                }
                
                if(endDate > lastLocalBarDateTime)
                {
                    var appendBarList = await dataProvider.GetHistoricalDataAsync(instrument, resolution, lastLocalBarDateTime, endDate);
                    repository.AppendData(instrument, resolution, appendBarList);
                    if (appendBarList.First().DateTime == localData.Last().DateTime)
                    {
                        localData.Remove(localData.Last());
                    }
                    localData.AddRange(appendBarList);
                }
                else if(endDate < lastLocalBarDateTime)
                {
                    int removeBarIndex = localData.FindIndex(p => p.DateTime >= endDate);
                    localData.RemoveRange(removeBarIndex, localData.Count - removeBarIndex);
                }

                return localData;
            }

            localData.AddRange(await dataProvider.GetHistoricalDataAsync(instrument, resolution, beginDate, endDate));
            repository.WriteData(instrument, resolution, localData);

            return localData;
        }

        //public IEnumerable<Bar> GetLocalData(Instrument instrument, Resolution resolution)
        //{
        //    return repository.ReadData(instrument, resolution);
        //}

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
            //dataProvider.Dispose();
        }

        private void logIn()
        {
            try
            {
                dataProvider.Login("U10D2386411", "1786", "http://www.fxcorporate.com/Hosts.jsp", "Demo");
            }
            catch (Exception e)
            {
                
                //Environment.Exit(0);
            }
        }
    }
}
