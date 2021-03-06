﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trading.Common;

namespace Trading.DataProviders.Common
{
    public interface IDataProvider 
    {
        Task<SessionStatusMessage> Login(params string [] loginData);
        void Logout();
        bool IsOnline { get; } 
        event EventHandler<SessionStatusChangedEventArgs> SessionStatusChanged;

        Task<IEnumerable<Bar>> GetHistoricalDataAsync(Instrument instrument, Resolution resolution,
            DateTime beginDateTime, DateTime endDateTime);

        void SubscribeToRealTime(string instrument);
        void UnsubscribeFromRealTime(string instrument);
        event EventHandler<RealTimeDataUpdatedEventArgs> RealTimeDataUpdated;

    }
}
