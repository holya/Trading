using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;
using Trading.DataBases.Common;
using Trading.DataProviders.Common;
using System.Xml.Linq;
using Trading.DataBases.XmlDataBase;
using Trading.DataManager.Common;

namespace Trading.DataManager
{
    public class DataManager : IDataManager
    {
        public event EventHandler<SessionStatusChangedEventArgs> SessionStatusChanged;
        public event EventHandler<RealTimeDataUpdatedEventArgs> RealTimeDataUpdated;

        private readonly IDataBase repository;
        private readonly IDataProvider dataProvider;

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

        public bool IsOnline => dataProvider.IsOnline;

        public void SubscribeRealTime(Instrument instrument) => dataProvider.SubscribeRealTime(instrument);

        public void UnsubscribeRealTime(Instrument instrument) => dataProvider.UnsubscribeRealTime(instrument);

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

            var barList = new List<Bar>();
            var firstLocalBarDateTime = localData.First().DateTime;
            var lastLocalBarDateTime = localData.Last().DateTime;


            //var bars = (beginDateLTFirstLocal, beginDateGTOrEqulFirstLocal, beginDateGTOrEqulLastLocal,
            //            endDateLTOrEqualFirstLocal, endDateLTOrEqualLastLocal, endDateGTLastLocal) switch
            //{

            //}

            if (beginDate < firstLocalBarDateTime)
            {
                if(endDate <= firstLocalBarDateTime)
                {
                    barList.AddRange(await dataProvider.GetHistoricalDataAsync(instrument, resolution, beginDate, firstLocalBarDateTime));

                    _ = await repository.PrependLocalData(instrument, resolution, barList);

                    var index = barList.FindIndex(bar => bar.DateTime >= endDate);

                    if(index > 0)
                        barList.RemoveRange(index, barList.Count - index);

                    return barList;
                }
                else if(endDate <= lastLocalBarDateTime)
                {
                    barList.AddRange(await dataProvider.GetHistoricalDataAsync(instrument, resolution, beginDate, firstLocalBarDateTime));
                    _ = await repository.PrependLocalData(instrument, resolution, barList);

                    barList.AddRange(await repository.ReadLocalDataAsync(instrument, resolution, firstLocalBarDateTime, endDate));

                    return barList;
                }
                else //endDate > lastLocalBarDateTime
                {
                    var firstbars = await dataProvider.GetHistoricalDataAsync(instrument, resolution, beginDate, firstLocalBarDateTime);
                    barList.AddRange(firstbars);
                    barList.AddRange(localData);
                    _ = await repository.PrependLocalData(instrument, resolution, firstbars);


                    var lastBars = (await dataProvider.GetHistoricalDataAsync(instrument, resolution, lastLocalBarDateTime, endDate)).ToList();
                    //barList.RemoveAt(barList.Count - 1);
                    lastBars.RemoveAt(0);
                    _ = await repository.AppendLocalData(instrument, resolution, lastBars);
                    barList.AddRange(lastBars);

                    return barList;
                }
            }
            else if(beginDate >= firstLocalBarDateTime && beginDate <= lastLocalBarDateTime)
            {
                if(lastLocalBarDateTime <= endDate)
                {
                    barList.AddRange(localData.FindAll(p => p.DateTime >= beginDate && p.DateTime <= endDate));

                    return barList;
                }
                else // endDate > lastLocalBarDateTime
                {

                }
            }
            else // beginDate > lastLocalBarDateTime
            {

            }

            return barList;
        }

        #endregion

        public void Dispose()
        {
            dataProvider.Logout();
        }
    }
}
