using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;
using Trading.Brokers.Fxcm;
using Trading.Databases.Interfaces;
using Trading.DataProviders.Interfaces;
using Trading.Databases.TextFileDataBase;
using Trading.Analyzers.Common;
using System.Xml.Linq;

namespace Trading.DataManager
{
    public class DataManager : IDisposable
    {
        TextDataBase db;
        FxcmWrapper dataProvider;

        public DataManager()
        {
            dataProvider = new FxcmWrapper();
            db = new TextDataBase();
            logIn();
        }


        public async Task<IEnumerable<Bar>> GetHistoricalDataAsync(Instrument instrument, Resolution resolution, 
            DateTime beginDate, DateTime endDate)
        {
            var k = dataProvider.GetServerTime();
            var data = db.ReadData(instrument, resolution).ToList();
            List<Bar> returnData = new List<Bar>();
            if (data.Count() != 0)
            {
                var firstBarDt = data.First().DateTime;
                var lastBarDt = data.Last().DateTime;

                if(beginDate < firstBarDt)
                {
                    returnData = await getDataAndWriteToDB(instrument, resolution, beginDate, dataProvider.GetServerTime());
                }
                else
                {
                    int i = data.FindIndex(p => p.DateTime >= beginDate);
                    data.RemoveRange(0, i - 1);
                    returnData.AddRange(data);
                }
                
                if(endDate > lastBarDt)
                {
                    returnData = await getDataAndWriteToDB(instrument, resolution, beginDate, dataProvider.GetServerTime());
                }
                else
                {
                    int i = data.FindIndex(p => p.DateTime >= endDate);
                    data.RemoveRange(i, data.Count - i);
                    returnData.AddRange(data);
                }

                return returnData;
            }

            var list = await getDataAndWriteToDB(instrument, resolution, beginDate, endDate);

            return list;
        }

        private async Task<List<Bar>> getDataAndWriteToDB(Instrument instrument, Resolution resolution, DateTime beginDate, DateTime endDate)
        {
            var list = await dataProvider.GetHistoricalDataAsync(instrument, resolution, beginDate, endDate);
            db.WriteData(instrument, resolution, list);
            return list.ToList();
        }

        public void Dispose()
        {
            dataProvider.Logout();
            dataProvider.Dispose();
        }

        private void logIn()
        {
            try
            {
                dataProvider.Login("U10D2386411", "1786", "http://www.fxcorporate.com/Hosts.jsp", "Demo");
            }
            catch (Exception e)
            {
                Environment.Exit(0);
            }
        }
    }

    public class SymbolsManager
    {
        private string docUri = "../../Symbols.xml";

        public SymbolsManager()
        {

        }

        public IEnumerable<string> GetForexPairsMajor()
        {
            return getForexPairs("major");
        }
        public IEnumerable<string> GetForexPairsMinor()
        {
            return getForexPairs("minor");
        }

        private IEnumerable<string> getForexPairs(string forexType)
        {
            var list = new List<string>();
            var doc = XDocument.Load(docUri);

            var elements = doc.Element("symbols").Element("forex").Element(forexType).Elements().Attributes("name");
            foreach (var v in elements)
                list.Add(v.Value);

            return list;
        }

    }
}
