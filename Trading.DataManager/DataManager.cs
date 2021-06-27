using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;
using Trading.Brokers.Fxcm;
using Trading.DataBases.Common;
using Trading.DataProviders.Common;
using Trading.Analyzers.Common;
using System.Xml.Linq;
using Trading.DataBases.XmlDataBase;
using Trading.DataManager.Common;
using System.Collections.Concurrent;

namespace Trading.DataManager
{
    public class DataManager : IDataManager
    {
        public event EventHandler<SessionStatusChangedEventArgs> SessionStatusChanged;
        public event EventHandler<RTDataUpdateEventArgs> RealTimeDataUpdated;

        private readonly IDataBase repository;
        private readonly IDataProvider dataProvider;

        private readonly ConcurrentDictionary<(Instrument, Resolution), DateTime> RealTimeBars = new ConcurrentDictionary<(Instrument, Resolution), DateTime>();

        public DataManager(IDataProvider dataProvider, IDataBase dataBase)
        {
            this.dataProvider = dataProvider;
            repository = dataBase;

            dataProvider.SessionStatusChanged += (object sender, SessionStatusChangedEventArgs e) =>
                SessionStatusChanged?.Invoke(sender, e);
            dataProvider.RealTimeDataUpdated += OnRealTimeDataUpdated;
        }

        #region IDataProvider implementations

        public async Task<SessionStatusMessage> Login(params string[] loginData) => await dataProvider.Login(loginData);

        public void Logout( ) => this.dataProvider.Logout();

        public bool IsOnline => dataProvider.IsOnline;

        public void SubscribeRealTime(Instrument instrument, Resolution resolution)
        {
            dataProvider.SubscribeRealTime(instrument);

            switch (resolution.TimeFrame)
            {
                case TimeFrame.Minute:
                    break;
                case TimeFrame.Hourly:
                    break;
                case TimeFrame.Daily:
                    break;
                case TimeFrame.Weekly:
                    break;
                case TimeFrame.Monthly:
                    break;
                case TimeFrame.Quarterly:
                    break;
                case TimeFrame.Yearly:
                    break;
                default:
                    break;
            }
            RealTimeBars.TryAdd((instrument, resolution), DateTime.Now);
        }

        public void UnsubscribeRealTime(Instrument instrument)
        {
            dataProvider.UnsubscribeRealTime(instrument);
        }
        public async Task<IEnumerable<Bar>> GetHistoricalDataAsync(Instrument instrument, Resolution resolution, 
            DateTime beginDate, DateTime endDate)
        
        {
            var localData =(List<Bar>)await repository.ReadLocalDataAsync(instrument, resolution, beginDate, endDate);

            if (!IsOnline)
                return localData;

            if(localData.Count() == 0 && !repository.FileExists(instrument, resolution))
            {
                localData.AddRange(await dataProvider.GetHistoricalDataAsync(instrument, resolution, beginDate, endDate));
                await repository.WriteLocalDataAsync(instrument, resolution, localData);
                return localData;
            }

            localData = (List<Bar>)await repository.ReadLocalDataAsync(instrument, resolution);

            var firstLocalBarDateTime = localData.First().DateTime;
            var lastLocalBarDateTime = localData.Last().DateTime;


            if(beginDate >= firstLocalBarDateTime && endDate < lastLocalBarDateTime)
            {
                var bars = localData.FindAll(p => p.DateTime >= beginDate && p.DateTime < endDate);
                return bars;
            }

            var barList = new List<Bar>();

            if(beginDate < firstLocalBarDateTime)
            {
                var bars = await dataProvider.GetHistoricalDataAsync(instrument, resolution, beginDate, firstLocalBarDateTime);
                await repository.PrependLocalData(instrument, resolution, bars);
            }
            if(endDate > lastLocalBarDateTime)
            {
                var bars = (await dataProvider.GetHistoricalDataAsync(instrument, resolution, lastLocalBarDateTime, endDate)).ToList();
                if (bars.Count() != 0)
                {
                    if (bars.First().DateTime == lastLocalBarDateTime)
                        bars.Remove(bars.First());
                    if(bars.Count() != 0)
                        await repository.AppendLocalData(instrument, resolution, bars);
                }
            }

            barList.AddRange(await repository.ReadLocalDataAsync(instrument, resolution, beginDate, endDate));

            return barList;
        }

        #endregion

        protected void OnRealTimeDataUpdated(object sender, RealTimeDataUpdatedEventArgs e)
        {


            //RealTimeDataUpdated?.Invoke(this, new RTDataUpdateEventArgs { })
        }

        public void Dispose()
        {
            dataProvider.Logout();
        }
    }
}
