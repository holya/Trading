using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;

namespace Trading.Databases.Interfaces
{
    public interface IDataBase
    {
        IEnumerable<Bar> LoadData(string symbol, Resolution resolution, DateTime from, DateTime to);
        void SaveData(string symbol, Resolution resolution, IEnumerable<Bar> barList);
    }
}
