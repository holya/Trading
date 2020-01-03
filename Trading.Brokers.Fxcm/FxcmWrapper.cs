using fxcore2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trading.Analyzers.Common;
using Trading.Common;
using Trading.DataProviders.Common;

namespace Trading.Brokers.Fxcm
{
    public class FxcmWrapper : IDataProvider, IDisposable
    {
        private List<string> realTimeInstrumentsList = new List<string>();

        public event EventHandler<SessionStatusChangedEventArgs> SessionStatusChanged = delegate { };
        //public bool IsOnline { get; private set; } = false;
        public bool IsOnline
        {
            get
            {
                return session.getSessionStatus() == O2GSessionStatusCode.Connected ? true : false;
            }
        }


        public event EventHandler<RealTimeDataUpdatedEventArgs> RealTimeDataUpdated = delegate { };

        private O2GSession session;
        private SessionStatusResponseListener sessionStatusResponseListener;

        private O2GTableManager tableMgr = null;
        O2GOffersTable offersTable = null;

        public FxcmWrapper()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            session = O2GTransport.createSession();
            sessionStatusResponseListener = new SessionStatusResponseListener(session, "", "");
            session.subscribeSessionStatus(sessionStatusResponseListener);
            session.SessionStatusChanged += Session_SessionStatusChanged;

        }

        public DateTime GetServerTime() => session.getServerTime();

        #region Login / Logout   -----------------------------------------------------------------------------

        public async Task<SessionStatusMessage> Login(params string[] loginData)
        {
            return await Task.Run(() => 
            {
                return doLogin(loginData);
            });
        }

        private SessionStatusMessage doLogin(string[] loginData)
        {
            session.subscribeSessionStatus(sessionStatusResponseListener);

            session.useTableManager(O2GTableManagerMode.Yes, null);

            session.login(loginData[0], loginData[1], loginData[2], loginData[3]);
            sessionStatusResponseListener.WaitEvents();

            if (sessionStatusResponseListener.Error)
                return new SessionStatusMessage(SessionStatusEnum.ERROR1, "FXCM: " + sessionStatusResponseListener.ErrorMessage);
            if (sessionStatusResponseListener.SessionStatus == O2GSessionStatusCode.SessionLost)
                return new SessionStatusMessage(SessionStatusEnum.ERROR2, "FXCM: Session Lost");
            if (sessionStatusResponseListener.SessionStatus == O2GSessionStatusCode.Unknown)
                return new SessionStatusMessage(SessionStatusEnum.ERROR3, "FXCM: Uknown Error");

            tableMgr = session.getTableManager();
            Task.Delay(1000);
            if (tableMgr == null)
                return new SessionStatusMessage(SessionStatusEnum.ERROR4, "FXCM: TableManage is null");

            O2GTableManagerStatus managerStatus = tableMgr.getStatus();

            int i = 0;
            while ((managerStatus == O2GTableManagerStatus.TablesLoading))
            {
                Thread.Sleep(200);
                managerStatus = tableMgr.getStatus();
                if (i > 9)
                    break;
            }
            if (managerStatus == O2GTableManagerStatus.TablesLoadFailed)
                return new SessionStatusMessage(SessionStatusEnum.ERROR3, "FXCM: TablesLoadFailed");

            offersTable = (O2GOffersTable)tableMgr.getTable(O2GTableType.Offers);
            offersTable.RowChanged += offersTableUpdated;

            return new SessionStatusMessage(SessionStatusEnum.Connected, "FXCM: Connected");
        }

        public void Logout()
        {
            session.logout();
            sessionStatusResponseListener.WaitEvents();
            session.unsubscribeSessionStatus(sessionStatusResponseListener);
        }

        private void Session_SessionStatusChanged(object sender, SessionStatusEventArgs e)
        {
            var sse = convertO2GSessionStatusCodeToSessionStatusEnum(e.SessionStatus);

            SessionStatusChanged?.Invoke(this, new SessionStatusChangedEventArgs() { SessionStatus = sse });
        }

        private SessionStatusEnum convertO2GSessionStatusCodeToSessionStatusEnum(O2GSessionStatusCode sessionStatusCode)
        {
            switch (sessionStatusCode)
            {
                case O2GSessionStatusCode.Connected:
                    return SessionStatusEnum.Connected;

                case O2GSessionStatusCode.Disconnected:
                    return SessionStatusEnum.Disconnected;

                case O2GSessionStatusCode.Connecting:
                    return SessionStatusEnum.MSG1; 

                case O2GSessionStatusCode.Disconnecting:
                    return SessionStatusEnum.MSG2;

                case O2GSessionStatusCode.Reconnecting:
                    return SessionStatusEnum.MSG3;

                case O2GSessionStatusCode.TradingSessionRequested:
                    return SessionStatusEnum.MSG4;

                case O2GSessionStatusCode.PriceSessionReconnecting:
                    return SessionStatusEnum.MSG5;

                case O2GSessionStatusCode.SessionLost:
                    return SessionStatusEnum.ERROR1;

                case O2GSessionStatusCode.Unknown:
                    return SessionStatusEnum.ERROR2;

                default:
                    return SessionStatusEnum.MSG10;
            }
        }

        #endregion  ---------------------------------------------------------------------------------------------------

        #region GetHistoricalDataAsync

        public async Task<IEnumerable<Bar>> GetHistoricalDataAsync(Instrument instrument, Resolution resolution, DateTime startDateTime, DateTime endDateTime)
        {
            Task<IEnumerable<Bar>> task = Task.Factory.StartNew(() =>
                    getHistoricalData(this.normalizeSymbol(instrument.Name), resolution, startDateTime, endDateTime));
            return await task;
        }

        private IEnumerable<Bar> getHistoricalData(string symbol, Resolution resolution, DateTime startDateTime, DateTime endDateTime)
        {
            GetHistoricalDataResponseListener responseListener = new GetHistoricalDataResponseListener(session);
            session.subscribeResponse(responseListener);
            List<FxBar> barList;
            try
            {
                barList = getHistoryPrices(session, symbol, convert_Resolution_To_string(resolution), startDateTime, endDateTime, 1000, responseListener);
                foreach (var bar in barList)
                    bar.EndDateTime = Utilities.GetEndDateTime(bar.DateTime, resolution);
            }
            catch (Exception e)
            {
                throw e;
            }

            if (resolution.TimeFrame == TimeFrame.Quarterly)
            {
                return normalizeToQuarterlyTimeFrame(barList);
            }

            session.unsubscribeResponse(responseListener);

            return barList;
        }


        private List<FxBar> getHistoryPrices(O2GSession session, string instrument, string timeFrame, DateTime startDateTime, DateTime endDateTime, int maxBars, GetHistoricalDataResponseListener responseListener)
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

        #endregion

        private void offersTableUpdated(object sender, RowEventArgs e)
        {
            O2GOfferTableRow otr = (O2GOfferTableRow)e.RowData;
            if (otr != null && realTimeInstrumentsList.Count > 0)
            {
                if(realTimeInstrumentsList.Contains(this.deNormalizeSymbol(otr.Instrument)))
                    //RealTimeDataUpdated?.Invoke(this, new Tuple<string ,double, double, DateTime, int>(otr.Instrument ,otr.Bid, otr.Ask, otr.Time, otr.Volume));
                    RealTimeDataUpdated?.Invoke(this, new RealTimeDataUpdatedEventArgs { Data = new Tuple<string, double, double, DateTime, int>(this.deNormalizeSymbol(otr.Instrument), otr.Bid, otr.Ask, otr.Time, otr.Volume) });


            }
        }

        private string normalizeSymbol(string symbol) => $"{symbol.Substring(0, 3)}/{symbol.Substring(3, 3)}";
        private string deNormalizeSymbol(string symbol) => $"{symbol.Substring(0, 3)}{symbol.Substring(4, 3)}";


        public void SubscribeToRealTime(string symbol)
        {
            if (!realTimeInstrumentsList.Contains(symbol))
                realTimeInstrumentsList.Add(symbol);
        }

        public void UnsubscribeFromRealTime(string symbol)
        {
            realTimeInstrumentsList.Remove(symbol);
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

        public void Dispose() => session.Dispose();

        public override string ToString()
        {
            return "Fxcm";
        }
    }
}
