using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;
namespace Trading.DataManager.Common
{
    public class RTTickUpdateEventArgs : EventArgs
    {
        public Instrument Instrument;
        public double Price;
        public int Volume;
        public DateTime DateTime;
    }
}
