using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.DataProviders.Common
{
    class SessionStatusChangedEventArgs : EventArgs
    {
        public SessionStatusEnum SessionStatus { get; set; }
    }
}
