using System.Collections.Generic;
using Trading.Common;

namespace Trading.DataBases.Interfaces
{
    public interface IDataBase
    {
        IEnumerable<Bar> ReadData(Instrument instrument, Resolution resolution);
        void WriteData(Instrument instrument, Resolution resolution, IEnumerable<Bar> barList);
    }
}
