using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;
namespace Trading.DataProviders.Common
{
    public class RealTimeDataUpdatedEventArgs : EventArgs
    {
        public RealTimeDataUpdatedEventArgs(Instrument instrument, double price, int volume, DateTime dateTime)
        {
            Instrument = instrument;
            Price = price;
            Volume = volume;
            DateTime = dateTime;
        }
        public Instrument Instrument;
        public double Price;
        public int Volume;
        public DateTime DateTime;
    }
}
