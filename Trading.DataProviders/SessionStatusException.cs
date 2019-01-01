using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.DataProviders.Common
{
    public class SessionStatusException : Exception
    {
        public SessionStatusEnum Status { get; set; }

        public SessionStatusException() : base()
        { }


        public SessionStatusException(string message) : base(message)
        { }

        public SessionStatusException(SessionStatusEnum status) : base()
        {
            this.Status = status;
        }

        public SessionStatusException(SessionStatusEnum status, string message) : base(message)
        {
            this.Status = status;
        }
    }
}
