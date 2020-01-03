using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trading.Common;
using Trading.DataProviders.Common;
using ActiveTickFeedLib;

namespace Trading.DataProviders.ActiveTick
{
    public class ActiveTick : IDataProvider
    {
        private ActiveTickFeedLib.Feed feed;

        private List<string> realTimeInstrumentsList = new List<string>();


        public bool IsOnline { get; private set; } = false;



        public event EventHandler<SessionStatusChangedEventArgs> SessionStatusChanged;
        public event EventHandler<RealTimeDataUpdatedEventArgs> RealTimeDataUpdated;

        //public event EventHandler<List<Bar>> HisoricalBarList;
        public ActiveTick()
        {
            this.feed = new ActiveTickFeedLib.Feed();

            feed.PrimaryServerHostname = "activetick1.activetick.com";
            feed.BackupServerHostname = "activetick2.activetick.com";
            feed.ServerPort = 443;
            feed.APIUserId = "{2b5d4cea-a861-4f27-90cc-a8bb6ee7f664}";

            //this.feed.OnServerSessionStateChange += OnServerSessionStateChange;
            this.feed.OnStreamQuote += Feed_OnStreamQuote;
        }

        private void Feed_OnStreamQuote(string symbol, byte quoteCondition, byte bidExchange, byte askExchange, double bidPrice, double askPrice, int bidSize, int askSize, DateTime quoteDatetime)
        {
            string data = "Stream Quote: " + symbol + "," + quoteCondition + "," + bidExchange + "," + askExchange + "," + bidPrice + "," + askPrice + "," + bidSize + "," + askSize + "," + quoteDatetime.ToString();

            RealTimeDataUpdated?.Invoke(this, new RealTimeDataUpdatedEventArgs { Data = new Tuple<string, double, double, DateTime, int>(symbol, bidPrice, askPrice, quoteDatetime, bidSize) });

        }

        //private void OnServerSessionStateChange(short prevSessionState, short newSessionState)
        //{
        //    var sse = convertATSessionStatusEnumToSessionStatusEnum((ATSessionStatusEnum)newSessionState);
        //    this.OnSessionStatusChanged(new SessionStatusChangedEventArgs() { SessionStatus = sse });
        //}

        private SessionStatusEnum convertATSessionStatusEnumToSessionStatusEnum(ATSessionStatusEnum sse)
        {
            switch (sse)
            {
                case ATSessionStatusEnum.ATSessionStatusDisconnected:
                    return SessionStatusEnum.Disconnected;
                case ATSessionStatusEnum.ATSessionStatusDisconnectedDuplicateLogin:
                    return SessionStatusEnum.ERROR1;
                case ATSessionStatusEnum.ATSessionStatusConnected:
                    return SessionStatusEnum.Connected;
                default:
                    return SessionStatusEnum.ERROR10;
            }
        }


        public async Task<SessionStatusMessage> Login(params string[] loginData)
        {
            TaskCompletionSource<SessionStatusMessage> tcs = new TaskCompletionSource<SessionStatusMessage>();
            this.feed.OnServerSessionStateChange += (short prevSessionState, short newSessionState) =>
            {
                switch ((ActiveTickFeedLib.ATSessionStatusEnum)newSessionState)
                {
                    case ATSessionStatusEnum.ATSessionStatusDisconnected:
                        this.IsOnline = false;
                        tcs.TrySetResult(new SessionStatusMessage(SessionStatusEnum.Disconnected, "ActiveTick: The Session has been disconnected!"));
                        break;

                    case ATSessionStatusEnum.ATSessionStatusDisconnectedDuplicateLogin:
                        this.IsOnline = false;
                        tcs.TrySetResult(new SessionStatusMessage(SessionStatusEnum.ERROR1, "ActiveTick: Duplicate Login!"));
                        break;

                    case ATSessionStatusEnum.ATSessionStatusConnected:
                        this.feed.SendLoginRequest(loginData[0], loginData[1]);
                        break;
                }

            };
            this.feed.OnLoginResponse += (short loginStatus, object entitlements) =>
            {
                switch ((ActiveTickFeedLib.ATLoginResponseEnum)loginStatus)
                {
                    case ATLoginResponseEnum.ATLoginResponseSuccess:
                        this.IsOnline = true;
                        tcs.SetResult(new SessionStatusMessage(SessionStatusEnum.Connected, "ActiveTick: Connected!"));
                        break;

                    case ATLoginResponseEnum.ATLoginResponseInvalidUserid:
                        tcs.TrySetResult(new SessionStatusMessage(SessionStatusEnum.WrongID, "ActiveTick: Wrong ID!"));
                        break;

                    case ATLoginResponseEnum.ATLoginResponseInvalidPassword:
                        tcs.TrySetResult(new SessionStatusMessage(SessionStatusEnum.WrongPassword, "ActiveTick: Wrong Password!"));
                        break;

                    case ATLoginResponseEnum.ATLoginResponseInvalidRequest:
                        tcs.TrySetResult(new SessionStatusMessage(SessionStatusEnum.ERROR2, "ActiveTick: Invalid Request!"));
                        break;

                    case ATLoginResponseEnum.ATLoginResponseLoginDenied:
                        tcs.TrySetResult(new SessionStatusMessage(SessionStatusEnum.ERROR3, "ActiveTick: Login Denied!"));
                        break;

                    case ATLoginResponseEnum.ATLoginResponseServerError:
                        tcs.TrySetResult(new SessionStatusMessage(SessionStatusEnum.ERROR4, "ActiveTick: Server Error!"));
                        break;
                }
            };

            this.feed.StartServerSession();
                
            return await tcs.Task;
        }

        public void Logout()
        {
            this.feed.StopServerSession();
        }
        public async Task<IEnumerable<Trading.Common.Bar>> GetHistoricalDataAsync(Trading.Common.Instrument instrument, Trading.Common.Resolution resolution, DateTime beginDateTime, DateTime endDateTime)
        {
            TaskCompletionSource<IEnumerable<Bar>> tcs = new TaskCompletionSource<IEnumerable<Bar>>();

            var symbol = instrument.Type == InstrumentType.Forex ? $"#{instrument.Name.Substring(0, 3)}/{instrument.Name.Substring(3, 3)}" :                                                        $"{instrument.Name}";

            switch (resolution.TimeFrame)
            {
                case TimeFrame.Minute:
                    var t = await this.getHistoricalDataAsync(symbol, ATBarHistoryEnum.ATBarHistoryIntraday,
                                    (short)resolution.Size, beginDateTime, endDateTime);
                    tcs.TrySetResult(t);
                    break;

                case TimeFrame.Hourly:
                    var minuteList = await this.getHistoricalDataAsync(symbol, ATBarHistoryEnum.ATBarHistoryIntraday,
                                    60, beginDateTime, endDateTime);

                    if (resolution.Size == 1)
                    {
                        tcs.TrySetResult(minuteList);
                        break;
                    }
                    var list = Utilities.CreatHourlyBarsFromMinuteBars(minuteList, resolution.Size);

                    tcs.TrySetResult(list);
                    break;
                case TimeFrame.Daily:
                    var l = await this.getHistoricalDataAsync(symbol, ATBarHistoryEnum.ATBarHistoryDaily,
                                    (short)resolution.Size, beginDateTime, endDateTime);
                    tcs.TrySetResult(l);
                    break;
                case TimeFrame.Weekly:
                    var li = await this.getHistoricalDataAsync(symbol, ATBarHistoryEnum.ATBarHistoryWeekly,
                                    (short)resolution.Size, beginDateTime, endDateTime);
                    tcs.TrySetResult(li);
                    break;
                case TimeFrame.Monthly:
                    var mList = await this.getHistoricalDataAsync(symbol, ATBarHistoryEnum.ATBarHistoryDaily,
                                    (short)resolution.Size, beginDateTime, endDateTime);

                    tcs.TrySetResult(Utilities.CreateMonthlyBarsFromDailyBars(mList, resolution.Size));
                    break;
                case TimeFrame.Quarterly:
                    var qList = await this.getHistoricalDataAsync(symbol, ATBarHistoryEnum.ATBarHistoryDaily,
                                    (short)resolution.Size, beginDateTime, endDateTime);

                    tcs.TrySetResult(Utilities.CreateMonthlyBarsFromDailyBars(qList, 3));
                    break;
                case TimeFrame.Yearly:
                    break;
                default:
                    break;
            }

            return await tcs.Task;
            //return await taskBarList;
        }

        private async Task<IEnumerable<Bar>> getHistoricalDataAsync(string symbol, ActiveTickFeedLib.ATBarHistoryEnum aTBarHistoryEnum, short intradayMinutes, DateTime beginDate, DateTime endDate)
        {
            List<Bar> barList = new List<Bar>();

            TaskCompletionSource<IEnumerable<Bar>> tcs = new TaskCompletionSource<IEnumerable<Bar>>();

            this.feed.OnBarHistoryResponse += (int originalRequestId, short barHistoryResponseCode, string instrument, short symbolStatusCode, object records) =>
            {
                switch ((ATSymbolStatusEnum)symbolStatusCode)
                {
                    case ATSymbolStatusEnum.ATSymbolStatusSuccess:
                        break;

                    case ATSymbolStatusEnum.ATSymbolStatusInvalid:
                        tcs.TrySetException( new HistoricalDataDownloadException(HistoricalDataDownloadMessageEnum.ERROR1, ATSymbolStatusEnum.ATSymbolStatusInvalid.ToString()));
                        break;
                    case ATSymbolStatusEnum.ATSymbolStatusUnavailable:
                        tcs.TrySetException(new HistoricalDataDownloadException(HistoricalDataDownloadMessageEnum.ERROR2, ATSymbolStatusEnum.ATSymbolStatusUnavailable.ToString()));
                        break;
                    case ATSymbolStatusEnum.ATSymbolStatusNoPermission:
                        tcs.TrySetException(new HistoricalDataDownloadException(HistoricalDataDownloadMessageEnum.ERROR3, ATSymbolStatusEnum.ATSymbolStatusNoPermission.ToString()));
                        break;
                    default:
                        break;
                }
                switch ((ActiveTickFeedLib.ATBarHistoryResponseEnum)barHistoryResponseCode)
                {
                    case ATBarHistoryResponseEnum.ATBarHistoryResponseSuccess:
                        if ((ActiveTickFeedLib.ATSymbolStatusEnum)symbolStatusCode == ATSymbolStatusEnum.ATSymbolStatusSuccess)
                        {
                            if (records != null)
                            {
                                string[] recordsArray = (string[])records;
                                foreach(var rec in recordsArray)
                                {
                                    var split = rec.Split(',');
                                    var d = split[0];
                                    DateTime dt = new DateTime(
                                                                Convert.ToInt32(d.Substring(0, 4)),
                                                                Convert.ToInt32(d.Substring(4, 2)),
                                                                Convert.ToInt32(d.Substring(6, 2)),
                                                                Convert.ToInt32(d.Substring(8, 2)),
                                                                Convert.ToInt32(d.Substring(10, 2)),
                                                                Convert.ToInt32(d.Substring(12, 2)),
                                                                Convert.ToInt32(d.Substring(14, 3)));
                                    barList.Add(new Bar
                                    {
                                        Open = Convert.ToDouble(split[1]),
                                        High = Convert.ToDouble(split[2]),
                                        Low = Convert.ToDouble(split[3]),
                                        Close = Convert.ToDouble(split[4]),
                                        Volume = Convert.ToDouble(split[5]),
                                        DateTime = dt,
                                        EndDateTime = dt
                                    });
                                }
                            }
                        }
                        tcs.TrySetResult(barList);
                        break;

                    case ATBarHistoryResponseEnum.ATBarHistoryResponseMaxLimitReached:
                        tcs.TrySetException(new HistoricalDataDownloadException(HistoricalDataDownloadMessageEnum.MSG1, ATBarHistoryResponseEnum.ATBarHistoryResponseMaxLimitReached.ToString()));
                        break;

                    case ATBarHistoryResponseEnum.ATBarHistoryResponseDenied:
                        tcs.TrySetException(new HistoricalDataDownloadException(HistoricalDataDownloadMessageEnum.MSG1, ATBarHistoryResponseEnum.ATBarHistoryResponseMaxLimitReached.ToString()));
                        break;

                    case ATBarHistoryResponseEnum.ATBarHistoryResponseInvalidRequest:
                        tcs.TrySetException(new HistoricalDataDownloadException(HistoricalDataDownloadMessageEnum.MSG1, ATBarHistoryResponseEnum.ATBarHistoryResponseInvalidRequest.ToString()));
                        break;
                }
            };

            int requestId = this.feed.SendBarHistoryRequest(symbol.ToUpper(), (short)aTBarHistoryEnum, intradayMinutes, beginDate, endDate);

            return  await tcs.Task;
        }

        public void SubscribeToRealTime(string instrument)
        {
            if (!realTimeInstrumentsList.Contains(instrument))
                realTimeInstrumentsList.Add(instrument);

            string[] symbols = new string[1] { instrument.ToUpper() };
            //symbols[0] = instrument.ToUpper();

            int requestId = this.feed.SendQuoteStreamRequest(symbols, (short)ActiveTickFeedLib.ATStreamRequestEnum.ATStreamRequestSubscribe);
        }

        public void UnsubscribeFromRealTime(string instrument)
        {
            realTimeInstrumentsList.Remove(instrument);
        }

        protected void OnSessionStatusChanged(SessionStatusChangedEventArgs args)
        {
            SessionStatusChanged?.Invoke(this, args);
        }

        public override string ToString() => "ActiveTick";
    }
}