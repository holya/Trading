using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;
using Trading.DataProviders.Interfaces;
using Trading.Databases.Interfaces;

namespace Trading.DataManager
{
    public class DataManager
    {
        void UpdateFileData(IEnumerable<Bar> readData, Resolution resolution, DateTime latestDate)
        {
            //should recieve read data from ReadData() and evaluate to what date append
            // StreamWriter(path, TRUE) the boolean parameter appends if the file exits hence true
        }
    }
}
