using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;
using Trading.Common.Instrument;

namespace Trading.Databases.Interfaces
{
    public interface IDataBase
    {
        IEnumerable<Bar> ReadData(Instrument instrument, Resolution resolution);
        void WriteData(Instrument instrument, Resolution resolution, IEnumerable<Bar> barList);
    }
}
