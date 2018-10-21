using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;
using Trading.Databases.Interfaces;
using Trading.Utilities;

namespace Trading.Databases.TextFileDataBase
{
    public class TextDataBase : IDataBase
    {
        SymbolsManager symbolsManager = new SymbolsManager();

        public IEnumerable<Bar> ReadData(string symbol, Resolution resolution, DateTime from, DateTime to)
        {
            throw new NotImplementedException();
        }

        public void WriteData(string symbol, Resolution resolution, IEnumerable<Bar> barList)
        {
            throw new NotImplementedException();
        }



        private IEnumerable<string> NormalizeSymbolsForDirectory(IEnumerable<string> pairs)
        {
            var refinedSymbol = new List<string>();
            char[] temp = new char[7];
            for (int i = 0; i < pairs.Count(); i++)
            {
                temp = pairs.ElementAt(i).ToCharArray();
                temp[3] = '_';
                string newSymbol = new string(temp);
                refinedSymbol.Add(newSymbol);
            }
            return refinedSymbol;
        }

        public void DirectoryFolderCheck()
        {
            string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string instrumentsDirectory = projectDirectory + "\\Instruments\\Forex";
            Directory.CreateDirectory(instrumentsDirectory);
            var totalPairs = symbolsManager.GetForexPairsMajor().Concat(symbolsManager.GetForexPairsMinor());

            totalPairs = NormalizeSymbolsForDirectory(totalPairs);
            foreach (var v in totalPairs)
            {
                string symbolsDirectory = instrumentsDirectory + $"/{v}";
                Directory.CreateDirectory(symbolsDirectory);
            }

        }
    }
}
