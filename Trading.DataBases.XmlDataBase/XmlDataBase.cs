using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Threading.Tasks;
using Trading.Common;
using Trading.DataBases.Interfaces;

namespace Trading.DataBases.XmlDataBase
{
    public class XmlDataBase : IDataBase
    {
        public string root = "C:\\DataBase\\XML\\";

        public IEnumerable<Bar> ReadData(Instrument instrument, Resolution resolution)
        {
            throw new NotImplementedException();
        }

        public void WriteData(Instrument instrument, Resolution resolution, IEnumerable<Bar> barList)
        {

        }

        public void PrependData(Instrument instrument, Resolution resolution, IEnumerable<Bar> barList)
        {

        }

        public void AppendData(Instrument instrument, Resolution resolution, IEnumerable<Bar> barList)
        {

        }

        private string getFullPath(Instrument instrument, Resolution resolution)
        {
            string directoryName = $"{getSymbolFolderName(instrument)}";
            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);

            string fileFullPath = $"{directoryName}\\{getFileName(resolution)}.xml";
            return fileFullPath;
        }

        private string getSymbolFolderName(Instrument instrument)
        {
            string path = $"{root}{instrument.Type.ToString()}";
            if (instrument.Type == InstrumentType.Forex)
                path += $"\\{instrument.Name.Substring(0, 3)}{instrument.Name.Substring(4, 3)}";
            else
                path += $"\\{instrument.Name}";

            return path;
        }

        private string getFileName(Resolution resolution) => $"{resolution.TimeFrame.ToString()}_{resolution.Size}";
    }

}
