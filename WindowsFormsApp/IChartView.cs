using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;

namespace WindowsFormsApp
{
    interface IChartView
    {       
        Instrument Instrument { get; set; }
        Resolution Resolution { get; set; }
        DateTime FromDateTime { get; set; }
        DateTime ToDateTime { get; set; }
        void AddBarList(IEnumerable<Bar> barList);
        void AddBar(Bar bar);
        void AddTick(double price, int volume, DateTime dateTime);
        void Reset( );
    }
}
