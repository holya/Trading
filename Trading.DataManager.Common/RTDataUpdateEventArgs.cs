using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;
namespace Trading.DataManager.Common
{
    public class RTDataUpdateEventArgs : EventArgs
    {
        public Instrument Instrument;
        public Resolution Resolution;
        public double Price;
        public DateTime DateTime;
        public bool IsNewBar;
    }
}
