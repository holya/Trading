using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.DataProviders.Common
{
    public enum SessionStatusEnum
    {
        Connected = 0,
        Disconnected = 1,
        SessionLost = 2,
        MSG1 = 3,
        MSG2 = 4,
        MSG3 = 5,
        MSG4 = 6,
        MSG5 = 7,
        MSG6 = 8,
        MSG7 = 9,
        MSG8 = 10
        //Disconnected = 0,
        //Connecting = 1,
        //TradingSessionRequested = 2,
        //Connected = 3,
        //Reconnecting = 4,
        //Disconnecting = 5,
        //SessionLost = 6,
        //PriceSessionReconnecting = 7,
        //Unknown = 8
    }
}
