using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.Brokers.Fxcm
{
    public enum SessionStatusEnum
    {
        Connected,
        Connecting,
        Disconnected,
        Disconnecting,
        Reconnecting,
        PriceSessionReconnecting,
        TradingSessionRequested,
        SessionLost,
        Unknown        
    }
}
