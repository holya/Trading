using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;
using Trading.Databases.Interfaces;

namespace Trading.Databases.TextFileDataBase
{
    public class TextDataBase : IDataBase
    {
        public IEnumerable<Bar> LoadData(string symbol, Resolution resolution, DateTime from, DateTime to)
        {
            throw new NotImplementedException();
        }

        public void SaveData(string symbol, Resolution resolution, IEnumerable<Bar> barList)
        {
            throw new NotImplementedException();
        }
    }
}
