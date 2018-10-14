using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;
using Trading.Databases.Interfaces;
using Trading.Utilities;

namespace TextFileDataBase
{
    public class TextDataBase : IDataBase
    {
        SymbolsManager symbolsManager = new SymbolsManager();

        public IEnumerable<Bar> LoadData(string symbol, Resolution resolution, DateTime from, DateTime to)
        {
            throw new NotImplementedException();
        }

        public void SaveData(string symbol, Resolution resolution, IEnumerable<Bar> barList)
        {
            throw new NotImplementedException();
        }

        public void DirectoryFolderCheck()
        {
            string projectDirectory = System.IO.Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string instrumentsDirectory = projectDirectory + "/Instruments/Forex";
            System.IO.Directory.CreateDirectory(instrumentsDirectory);

            var majorPairs = symbolsManager.GetForexPairsMajor();
            foreach (var v in majorPairs)
            {
                string majorsDirectory = instrumentsDirectory + $"/Majors/{v}";
                System.IO.Directory.CreateDirectory(majorsDirectory);
            }

            var minorPairs = symbolsManager.GetForexPairsMinor();
            foreach (var i in minorPairs)
            {
                string minorsDirectory = instrumentsDirectory + $"/Minors/{i}";
                System.IO.Directory.CreateDirectory(minorsDirectory);
            }
        }


    }
}
