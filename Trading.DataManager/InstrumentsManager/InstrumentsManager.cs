using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Trading.DataManager
{
    public class InstrumentsManager : IInstrumentsManager
    {
        private string docUri = "../../Symbols.xml";

        public InstrumentsManager()
        {

        }

        public IEnumerable<string> GetForexPairs(ForexTypes forexType)
        {
            var list = new List<string>();
            var doc = XDocument.Load(docUri);

            var elements = doc.Root.Element("forex").Element(forexType.ToString()).Elements().Attributes("name");
            foreach (var v in elements)
                list.Add(v.Value);

            return list;
        }
    }
}
