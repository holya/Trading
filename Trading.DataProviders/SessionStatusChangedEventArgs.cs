using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.DataProviders.Common
{
    public class SessionStatusChangedEventArgs : EventArgs
    {
        public SessionStatusEnum SessionStatus { get; set; }
        public string Message { get; set; }
    }
}
