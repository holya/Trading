using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;

namespace Trading.DataProviders.Common
{
    public interface IDataProvider
    {
        Task<IEnumerable<Bar>> GetHistoricalDataAsync(Instrument instrument, Resolution resolution,
            DateTime beginDateTime, DateTime endDateTime);

        void SubscribeToRealTime(string instrument);
        void UnsubscribeToRealTime(string instrument);
        event EventHandler<DataUpdatedEventArgs> DataUpdated;
        void Login(params string [] loginData);

        void Logout();
    }
}
