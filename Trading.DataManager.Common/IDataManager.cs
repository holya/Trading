using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;
using Trading.DataBases.Common;
using Trading.DataProviders.Common;

namespace Trading.DataManager.Common
{
    public interface IDataManager : IDisposable
    {
        Task<SessionStatusMessage> Login(params string[] loginData);
        void Logout( );
        bool IsOnline { get; }
        event EventHandler<SessionStatusChangedEventArgs> SessionStatusChanged;

        Task<IEnumerable<Bar>> GetHistoricalDataAsync(Instrument instrument, Resolution resolution,
            DateTime beginDateTime, DateTime endDateTime);

        void SubscribeRealTime(Instrument instrument, Resolution resolution);
        void UnsubscribeRealTime(Instrument instrument);
        event EventHandler<RTDataUpdateEventArgs> RealTimeDataUpdated;

    }
}
