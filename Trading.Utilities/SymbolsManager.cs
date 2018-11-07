using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Trading.Utilities
{
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
