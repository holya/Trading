using fxcore2;
using System;
using System.Collections.Generic;
using System.Threading;
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
            if(sessionStatusResponseListener.Error)
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
                barList = GetHistoryPrices(session, symbol, "D1", startDateTime, endDateTime, 1000, responseListener);
            }
            catch(Exception e)
            {
                throw e;
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
                throw new  Exception($"{responseListener.Error}");
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

        private string convertTimeFrameEnumToString(TimeFrame timeFrame)
        {
            return string.Empty;
        }

        private SessionStatusEnum convert_O2GSessionStatusCode_To_SessionStatusEnum(O2GSessionStatusCode statusCode)
        {
            return (SessionStatusEnum)statusCode;
        }

    }
}
