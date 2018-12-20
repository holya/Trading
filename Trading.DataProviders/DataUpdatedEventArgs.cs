using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.DataProviders.Common
{
    public class RealTimeDataUpdatedEventArgs : EventArgs
    {
        public object Data { get; set; }
    }
}
