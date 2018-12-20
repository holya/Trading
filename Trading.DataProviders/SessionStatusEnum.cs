using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.DataProviders.Common
{
    public enum SessionStatusEnum
    {
        //Connected,
        //Connecting = 1,
        //Disconnected = 0,
        //Disconnecting,
        //Reconnecting,
        //PriceSessionReconnecting,
        //TradingSessionRequested,
        //SessionLost,
        //Unknown
        Disconnected = 0,
        Connecting = 1,
        TradingSessionRequested = 2,
        Connected = 3,
        Reconnecting = 4,
        Disconnecting = 5,
        SessionLost = 6,
        PriceSessionReconnecting = 7,
        Unknown = 8
    }
}
