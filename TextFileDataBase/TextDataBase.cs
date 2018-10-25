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
        string root = "C:\\DataBase\\Forex\\";

        public IEnumerable<Bar> ReadData(string symbol, Resolution resolution)
        {
            List<Bar> barList = new List<Bar>();

            string directoryName = root + getSymbolFolderName(symbol);
            if (!Directory.Exists(directoryName))
                return barList;

            string fn = getFileName(resolution);
            string fileFullPath = $"{directoryName}\\{fn}.txt";
            if (!File.Exists(fileFullPath))
                return barList;

            using (StreamReader sr = new StreamReader(fileFullPath))
            {
                string line;
                while((line = sr.ReadLine()) != null)
                {
                    string[] str = line.Split(new char[] { ',' });
                    if(str.Count() > 7)
                    {
                        FxBar b = new FxBar(Convert.ToDouble(str[0]), Convert.ToDouble(str[1]), Convert.ToDouble(str[2]), Convert.ToDouble(str[3]), Convert.ToDouble(str[4]), Convert.ToDouble(str[5]), Convert.ToDouble(str[6]), Convert.ToDouble(str[7]), Convert.ToDouble(str[8]), Convert.ToDateTime(str[9]), Convert.ToDateTime(str[10]));

                        barList.Add(b);
                    }
                }
            }

            return barList;
        }

        public void WriteData(string symbol, Resolution resolution, IEnumerable<Bar> barList)
        {
            string directoryName = root + getSymbolFolderName(symbol);
            bool directoryExists = Directory.Exists(directoryName);
            if (!directoryExists)
                Directory.CreateDirectory(directoryName);

            string fn = getFileName(resolution);
            string fileFullPath = $"{directoryName}\\{fn}.txt";

            using(StreamWriter sw = new StreamWriter(fileFullPath))
            {
                foreach (var bar in barList)
                    sw.WriteLine(bar.ToString());
            }
        }

        private string getSymbolFolderName(string symbol)
        {
            return symbol.Substring(0, 3) + symbol.Substring(4, 3);
        }
        private string getFileName(Resolution resolution)
        {
            return $"{resolution.TimeFrame.ToString()}_{resolution.Size}";
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
