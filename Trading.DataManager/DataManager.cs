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
using Trading.DataManager.Common;

namespace Trading.DataManager
{
    public class DataManager : IDataManager
    {
        public event EventHandler<SessionStatusChangedEventArgs> SessionStatusChanged;
        public event EventHandler<RealTimeDataUpdatedEventArgs> RealTimeDataUpdated;

        private IDataBase repository;
        private IDataProvider dataProvider;

        public DataManager(IDataProvider dataProvider, IDataBase dataBase)
        {
            this.dataProvider = dataProvider;
            repository = dataBase;

            dataProvider.SessionStatusChanged += (object sender, SessionStatusChangedEventArgs e) =>
                SessionStatusChanged?.Invoke(sender, e);
            dataProvider.RealTimeDataUpdated += (object sender, RealTimeDataUpdatedEventArgs e) =>
                RealTimeDataUpdated?.Invoke(sender, e);
        }

        #region IDataProvider implementations

        public async Task<SessionStatusMessage> Login(params string[] loginData)
        {
            return await dataProvider.Login(loginData);
        }

        public void Logout()
        {
            this.dataProvider.Logout();
        }

        public bool IsOnline
        {
            get
            {
                return dataProvider.IsOnline;
            }
        }

        public void SubscribeToRealTime(string instrument)
        {
            dataProvider.SubscribeToRealTime(instrument);
        }

        public void UnsubscribeFromRealTime(string instrument)
        {
            dataProvider.UnsubscribeFromRealTime(instrument);
        }


        
        public async Task<IEnumerable<Bar>> GetHistoricalDataAsync(Instrument instrument, Resolution resolution, 
            DateTime beginDate, DateTime endDate)
        {
            var localData = repository.ReadLocalData(instrument, resolution, beginDate, endDate).ToList();



            if (!IsOnline)
                return localData;

            if(localData.Count == 0)
            {
                localData.AddRange(await dataProvider.GetHistoricalDataAsync(instrument, resolution, beginDate, endDate));
                repository.WriteLocalData(instrument, resolution, localData);
                return localData;
            }


            var firstLocalBarDateTime = localData.First().DateTime;
            var lastLocalBarDateTime = localData.Last().DateTime;

            if(beginDate < firstLocalBarDateTime)
            {
                var prependBarList = (await dataProvider.GetHistoricalDataAsync(instrument, resolution, beginDate, firstLocalBarDateTime)).ToList();

                if (prependBarList.Count > 0 && prependBarList.Last().DateTime == firstLocalBarDateTime)
                    prependBarList.Remove(prependBarList.Last());
                if (prependBarList.Count() > 0)
                {
                    repository.PrependLocalData(instrument, resolution, prependBarList);
                    localData.InsertRange(0, prependBarList);
                }
            }
            else if(beginDate > firstLocalBarDateTime)
            {
                if(beginDate >= lastLocalBarDateTime)
                {
                    var list = (await dataProvider.GetHistoricalDataAsync(instrument, resolution, lastLocalBarDateTime, endDate)).ToList();
                    repository.AppendLocalData(instrument, resolution, list);

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
                repository.AppendLocalData(instrument, resolution, appendBarList);
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

        #endregion

        #region IDataBase implementations

        public IEnumerable<Bar> ReadLocalData(Instrument instrument, Resolution resolution, DateTime fromDate, DateTime toDate)
        {
            return this.repository.ReadLocalData(instrument, resolution, fromDate, toDate);
        }

        public void WriteLocalData(Instrument instrument, Resolution resolution, IEnumerable<Bar> barList)
        {
            this.repository.WriteLocalData(instrument, resolution, barList);
        }

        public void PrependLocalData(Instrument instrument, Resolution resolution, IEnumerable<Bar> barList)
        {
            this.repository.PrependLocalData(instrument, resolution, barList);
        }

        public void AppendLocalData(Instrument instrument, Resolution resolution, IEnumerable<Bar> barList)
        {
            this.repository.PrependLocalData(instrument, resolution, barList);
        }

        #endregion

        public void Dispose()
        {
            dataProvider.Logout();
        }
    }
}
