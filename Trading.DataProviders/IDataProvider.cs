﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;
using System.Threading.Tasks;

namespace Trading.DataProviders.Common
{
    public interface IDataProvider
    {
        Task Login(params string [] loginData);
        void Logout();
        bool IsOnline { get; }
        event EventHandler<SessionStatusChangedEventArgs> SessionStatusChanged;

        Task<IEnumerable<Bar>> GetHistoricalDataAsync(Instrument instrument, Resolution resolution,
            DateTime beginDateTime, DateTime endDateTime);

        void SubscribeToRealTime(string instrument);
        void UnsubscribeToRealTime(string instrument);
        event EventHandler<RealTimeDataUpdatedEventArgs> RealTimeDataUpdated;

    }
}
