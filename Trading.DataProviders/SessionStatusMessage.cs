using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.DataProviders.Common
{
    public class SessionStatusMessage
    {
        public SessionStatusEnum SessionStatus { get; private set; }
        public string Message { get; private set; }
        public SessionStatusMessage(SessionStatusEnum sessionStatus, string message)
        {
            this.SessionStatus = sessionStatus;
            this.Message = message;
        }
    }
}
