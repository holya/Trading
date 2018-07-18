using fxcore2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;
using Trading.DataProviders;

namespace Trading.Brokers.Fxcm
{
    public class FxcmWrapper : IDataProvider
    {
        public event EventHandler<SessionStatusEnum> SessionStatusChanged = delegate { };
        public SessionStatusEnum SessionStatusEnum { get; private set; } = SessionStatusEnum.Disconnected;

        private O2GSession session;
        private SessionStatusResponseListener sessionStatusResponseListener;

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


        public IEnumerable<Bar> GetHistoricalData(string symbol, Resolution resolution, DateTime startDateTime, DateTime endDateTime)
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

            if(resolution.TimeFrame == TimeFrame.Quarterly)
            {
                List<FxBar> quarterlyBarList = new List<FxBar>();

                var index = 0;

                List<FxBar> tempList; 

                while(index < barList.Count)
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

            return barList;
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
            return (SessionStatusEnum)statusCode;
        }

    }
}
