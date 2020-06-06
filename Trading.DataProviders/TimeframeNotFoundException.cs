using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.DataProviders.Common
{
    public class TimeframeNotFoundException : Exception
    {
        public TimeframeNotFoundException(string message) : base(message)
        {

        }
    }
}
