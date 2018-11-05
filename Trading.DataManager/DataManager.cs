using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;
using Trading.Brokers.Fxcm;
using Trading.DataProviders.Interfaces;
using Trading.Databases.TextFileDataBase;
using Trading.Common.Instrument;

namespace Trading.DataManager
{
    public class DataManager
    {
        TextDataBase db = new TextDataBase();

        FxcmWrapper fxm = new FxcmWrapper();

        public IEnumerable<Bar> RespondToUserDataRequest(Instrument instrument, Resolution resolution)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Bar> DataDownload(Instrument instrument, Resolution resolution)
        {
            throw new NotImplementedException();
        }

    }
}
