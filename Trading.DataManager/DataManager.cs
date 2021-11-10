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
        public event EventHandler<SessionStatusChangedEventArgs> SessionStatusChanged = delegate { };
        public event EventHandler<RTTickUpdateEventArgs> RealTimeTickUpdated = delegate { };
        public event EventHandler<RTNewBarEventArgs> RealTimeNewBarAdded = delegate { };

        private readonly IDataBase repository;
        private readonly IDataProvider dataProvider;

        private readonly ConcurrentDictionary<(Instrument, Resolution), DateTime> RealTimeBars = new ConcurrentDictionary<(Instrument, Resolution), DateTime>();

        public DataManager(IDataProvider dataProvider, IDataBase dataBase)
        {
            this.dataProvider = dataProvider;
            repository = dataBase;

            dataProvider.SessionStatusChanged += (object sender, SessionStatusChangedEventArgs e) =>
                SessionStatusChanged?.Invoke(sender, e);
            dataProvider.RealTimeDataUpdated += OnDataProviderRealTimeDataUpdated;
        }

        #region IDataProvider implementations

        public async Task<SessionStatusMessage> Login(params string[] loginData) => await dataProvider.Login(loginData);

        public void Logout( ) => this.dataProvider.Logout();

        public bool IsOnline => dataProvider.IsOnline;

        public void SubscribeRealTime(Instrument instrument, Resolution resolution)
        {
            dataProvider.SubscribeRealTime(instrument);

            DateTime dt = Utilities.NormalizeAndGetEndDateTime(DateTime.UtcNow, resolution);

            RealTimeBars.AddOrUpdate((instrument, resolution), dt, (newKy, newVal) => dt);
        }

        public void UnsubscribeRealTime(Instrument instrument, Resolution resolution)
        {
            dataProvider.UnsubscribeRealTime(instrument);

            foreach (var item in RealTimeBars)
                if (item.Key.Item1.Name == instrument.Name && item.Key.Item2.TimeFrame == resolution.TimeFrame)
                    RealTimeBars.TryRemove(item.Key, out _);
        }
        public async Task<IEnumerable<Bar>> GetHistoricalDataAsync(Instrument instrument, Resolution resolution, 
            DateTime beginDate, DateTime endDate)
        {
            var localData =(List<Bar>)await repository.ReadLocalDataAsync(instrument, resolution, beginDate, endDate);

            /////// not good
            if (!IsOnline)
                return localData;

            if(localData.Count() == 0 && !repository.FileExists(instrument, resolution))
            {
                localData.AddRange(await dataProvider.GetHistoricalDataAsync(instrument, resolution, beginDate, endDate));
                await repository.WriteLocalDataAsync(instrument, resolution, localData);
                return localData;
            }
            /////////////
            ///

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

        protected void OnDataProviderRealTimeDataUpdated(object sender, RealTimeDataUpdatedEventArgs e)
        {
            bool sent = false;

            foreach (var item in RealTimeBars)
            {
                if (e.DateTime >= item.Value)
                {
                    var newDt = Utilities.GetEndDateTime(item.Value, item.Key.Item2);

                    RealTimeBars.TryUpdate((item.Key), newDt, item.Value);

                    RealTimeNewBarAdded?.Invoke(this, new RTNewBarEventArgs { Instrument = e.Instrument, Resolution = item.Key.Item2, Bar = new Bar(e.Price, e.Price, e.Price, e.Price, e.Volume, e.DateTime) });
                }
                else if (!sent)
                {
                    RealTimeTickUpdated?.Invoke(this, new RTTickUpdateEventArgs { Instrument = e.Instrument, Price = e.Price, Volume = e.Volume, DateTime = e.DateTime });
                    sent = true;
                }
            }
        }

        public void Dispose( ) => dataProvider.Logout();
    }
}
