using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;
namespace Trading.DataManager.Common
{
    public class RTNewBarEventArgs : EventArgs
    {
        public Instrument Instrument;
        public Resolution Resolution;
        public Bar Bar;
    }
}
