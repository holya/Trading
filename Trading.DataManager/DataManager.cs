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
            if (LocalDataReport(instrument, resolution) == true)
            {
                var localData = db.ReadData(instrument, resolution);
                return localData;
            }
            else
            {
                var downloadedData = DataDownload(instrument, resolution);
                return downloadedData;
            }
        }

        public IEnumerable<Bar> DataDownload(Instrument instrument, Resolution resolution)
        {
            throw new NotImplementedException();
        }

        private bool LocalDataReport(Instrument instrument, Resolution resolution)
        {
            var temp = db.ReadData(instrument, resolution);

            if (temp.Count() != 0)
                return true;
            else return false;
        }
    }
}
