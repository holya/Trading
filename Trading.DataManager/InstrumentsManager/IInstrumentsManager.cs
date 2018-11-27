using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.DataManager
{
    interface IInstrumentsManager
    {
        IEnumerable<string> GetForexPairs(ForexTypes forexType);
    }
}
