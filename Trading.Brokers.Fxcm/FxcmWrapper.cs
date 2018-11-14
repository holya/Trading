using fxcore2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trading.Analyzers.Common;
using Trading.Common;
using Trading.DataProviders.Interfaces;

namespace Trading.Brokers.Fxcm
{
    public class FxcmWrapper : IDataProvider, IDisposable
    {
        private List<string> realTimeInstruments = new List<string>();

        public event EventHandler<SessionStatusEnum> SessionStatusChanged = delegate { };
        public SessionStatusEnum SessionStatusEnum { get; private set; } = SessionStatusEnum.Disconnected;

        public event EventHandler<Tuple<string, double, double, DateTime, int>> OffersTableUpdated = delegate { };

        private O2GSession session;
        private SessionStatusResponseListener sessionStatusResponseListener;

        private O2GTableManager tableMgr = null;
        O2GOffersTable offersTable = null;

        public FxcmWrapper()
        {
            session = O2GTransport.createSession();

            sessionStatusResponseListener = new SessionStatusResponseListener(session, "", "");
            session.subscribeSessionStatus(sessionStatusResponseListener);


            session.SessionStatusChanged += Session_SessionStatusChanged;
        }

        public DateTime GetServerTime()
        {
            return session.getServerTime();
        }

        #region Login / Logout
        public void Login(string userID, string password, string url, string connectionType)
        {
            session.useTableManager(O2GTableManagerMode.Yes, null);
            try
            {
                session.login(userID, password, url, connectionType);
                sessionStatusResponseListener.WaitEvents();
            }
            catch (Exception e)
            {
                throw e;
            }

            if (sessionStatusResponseListener.Error)
            {
                throw new Exception(sessionStatusResponseListener.ErrorMessage);
            }

            tableMgr = session.getTableManager();
            O2GTableManagerStatus managerStatus = tableMgr.getStatus();
            while (managerStatus == O2GTableManagerStatus.TablesLoading)
            {
                Thread.Sleep(50);
                managerStatus = tableMgr.getStatus();
            }
            if (managerStatus == O2GTableManagerStatus.TablesLoadFailed)
                throw new Exception("Could not load Table Manager."); 

            offersTable = (O2GOffersTable)tableMgr.getTable(O2GTableType.Offers);
            offersTable.RowChanged += OffersTable_RowChanged;
        }

        private void Session_SessionStatusChanged(object sender, SessionStatusEventArgs e)
        {
            SessionStatusEnum = (SessionStatusEnum)e.SessionStatus;
            SessionStatusChanged?.Invoke(this, SessionStatusEnum);
        }

        public void Logout()
        {
            session.logout();
            sessionStatusResponseListener.WaitEvents();
            session.unsubscribeSessionStatus(sessionStatusResponseListener);
        }
        #endregion

        private IEnumerable<Bar> GetHistoricalData(string symbol, Resolution resolution, DateTime startDateTime, DateTime endDateTime)
        {
            GetHistoricalDataResponseListener responseListener = new GetHistoricalDataResponseListener(session);
            session.subscribeResponse(responseListener);
            List<FxBar> barList;
            try
            {
                barList = GetHistoryPrices(session, symbol, convert_Resolution_To_string(resolution), startDateTime, endDateTime, 1000, responseListener);
            }
            catch (Exception e)
            {
                throw e;
            }

            if (resolution.TimeFrame == TimeFrame.Quarterly)
            {
                return normalizeToQuarterlyTimeFrame(barList);
            }

            return barList;
        }

        public void SubscribeToRealTime(Instrument instrument)
        {
            if (!realTimeInstruments.Contains(instrument.Name))
                realTimeInstruments.Add(instrument.Name);
        }

        public async Task<IEnumerable<Bar>> GetHistoricalDataAsync(Instrument instrument, Resolution resolution, DateTime startDateTime, DateTime endDateTime)
        {
            Task<IEnumerable<Bar>> task = Task.Factory.StartNew(() =>
            GetHistoricalData(instrument.Name, resolution, startDateTime, endDateTime));
            return await task;
        }

        private void OffersTable_RowChanged(object sender, RowEventArgs e)
        {
            O2GOfferTableRow otr = (O2GOfferTableRow)e.RowData;
            if (otr != null && realTimeInstruments.Count > 0)
            {
                if(realTimeInstruments.Contains(otr.Instrument))
                    OffersTableUpdated?.Invoke(this, new Tuple<string ,double, double, DateTime, int>(otr.Instrument ,otr.Bid, otr.Ask, otr.Time, otr.Volume));
            }
        }

        public void UnsubscibeRealTime(string symbol)
        {
            realTimeInstruments.Remove(symbol);
        }

        private static IEnumerable<Bar> normalizeToQuarterlyTimeFrame(List<FxBar> barList)
        {
            List<FxBar> quarterlyBarList = new List<FxBar>();

            var index = 0;

            List<FxBar> tempList;

            while (index < barList.Count)
            {
                int count = 3 - ((barList[index].DateTime.Month - 1) % 3);

                tempList = barList.GetRange(index, count);

                index = index + count;

                var open = tempList.First().Open;
                var askOpen = tempList.First().AskOpen;

                var close = tempList.Last().Close;
                var askClose = tempList.Last().AskClose;

                var high = tempList.Max(p => p.High);
                var askHigh = tempList.Max(p => p.AskHigh);

                var low = tempList.Min(p => p.Low);
                var askLow = tempList.Min((p) => p.AskLow);

                var volume = tempList.Sum(p => p.Volume);

                var dateTime = tempList.Last().DateTime;

                FxBar bar = new FxBar()
                {
                    Open = open,
                    AskOpen = askOpen,
                    High = high,
                    AskHigh = askHigh,
                    Low = low,
                    AskLow = askLow,
                    Close = close,
                    AskClose = askClose,
                    Volume = volume,
                    DateTime = dateTime
                };

                quarterlyBarList.Add(bar);
            }
            return quarterlyBarList;
        }

        private List<FxBar> GetHistoryPrices(O2GSession session, string instrument, string timeFrame, DateTime startDateTime, DateTime endDateTime, int maxBars, GetHistoricalDataResponseListener responseListener)
        {
            List<FxBar> barList = new List<FxBar>();

            O2GRequestFactory factory = session.getRequestFactory();

            O2GTimeframe timeframe = factory.Timeframes[timeFrame];
            if (timeframe == null)
            {
                throw new Exception(string.Format("Timeframe '{0}' is incorrect!", timeFrame));
            }

            O2GRequest request = factory.createMarketDataSnapshotRequestInstrument(instrument, timeframe, maxBars);

            factory.fillMarketDataSnapshotRequestTime(request, startDateTime, endDateTime, false);

            responseListener.SetRequestID(request.RequestID);
            session.sendRequest(request);

            if (!responseListener.WaitEvents())
            {
                throw new Exception($"{responseListener.Error}");
            }

            O2GResponse response = responseListener.GetResponse();

            if (response != null && response.Type == O2GResponseType.MarketDataSnapshot)
            {
                O2GResponseReaderFactory readerFactory = session.getResponseReaderFactory();
                if (readerFactory != null)
                {
                    O2GMarketDataSnapshotResponseReader reader = readerFactory.createMarketDataSnapshotReader(response);
                    if (reader.Count > 0)
                    {
                        for (int i = 0; i < reader.Count; i++)
                        {
                            barList.Add(new FxBar
                            {
                                Open = reader.getBidOpen(i),
                                High = reader.getBidHigh(i),
                                Low = reader.getBidLow(i),
                                Close = reader.getBidClose(i),
                                AskOpen = reader.getAskOpen(i),
                                AskHigh = reader.getAskHigh(i),
                                AskLow = reader.getAskLow(i),
                                AskClose = reader.getAskClose(i),
                                Volume = reader.getVolume(i),
                                DateTime = reader.getDate(i)
                            });
                        }
                    }
                }
            }

            return barList;
        }

        private string convert_Resolution_To_string(Resolution resolution)
        {
            var str = "";
            switch (resolution.TimeFrame)
            {
                case TimeFrame.Yearly:
                    if (resolution.Size == 1)
                        str = $"Y{resolution.Size}";
                    break;
                case TimeFrame.Quarterly:
                    if (resolution.Size == 1)
                        str = $"M{resolution.Size}";
                    break;
                case TimeFrame.Monthly:
                    if (resolution.Size == 1)
                        str = $"M{resolution.Size}";
                    break;
                case TimeFrame.Weekly:
                    if (resolution.Size == 1)
                        str = $"W{resolution.Size}";
                    break;
                case TimeFrame.Daily:
                    if (resolution.Size == 1)
                        str = $"D{resolution.Size}";
                    break;
                case TimeFrame.Hourly:
                    if (resolution.Size == 1 || resolution.Size == 2 || resolution.Size == 3 || resolution.Size == 4 
                        || resolution.Size == 6)
                        str = $"H{resolution.Size}";
                    break;
                case TimeFrame.Minute:
                    if(resolution.Size == 1 || resolution.Size == 5 || resolution.Size == 15 || resolution.Size == 30)
                        str = $"m{resolution.Size}"; 
                    break;
                default:
                    break;
            }

            return str;
        }

        private SessionStatusEnum convert_O2GSessionStatusCode_To_SessionStatusEnum(O2GSessionStatusCode statusCode)
        {
            switch (statusCode)
            {
                case O2GSessionStatusCode.Disconnected:
                    return SessionStatusEnum.Disconnected;

                case O2GSessionStatusCode.Connecting:
                    return SessionStatusEnum.Connecting;

                case O2GSessionStatusCode.TradingSessionRequested:
                    return SessionStatusEnum.TradingSessionRequested;

                case O2GSessionStatusCode.Connected:
                    return SessionStatusEnum.Connected;

                case O2GSessionStatusCode.Reconnecting:
                    return SessionStatusEnum.Reconnecting;

                case O2GSessionStatusCode.Disconnecting:
                    return SessionStatusEnum.Disconnecting;

                case O2GSessionStatusCode.SessionLost:
                    return SessionStatusEnum.SessionLost;

                case O2GSessionStatusCode.PriceSessionReconnecting:
                    return SessionStatusEnum.PriceSessionReconnecting;

                case O2GSessionStatusCode.Unknown:
                    return SessionStatusEnum.Unknown;

                default:
                    return SessionStatusEnum.Unknown;
            }
        }

        public void Dispose()
        {
            session.Dispose();
        }

        public void Login()
        {

        }
    }
}
