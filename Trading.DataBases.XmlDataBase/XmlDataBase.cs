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
            string fileFullPath = getFullPath(instrument, resolution);

            XElement barData;

            foreach (var bar in barList)
            {
                barData = new XElement("Bar Data",
                    new XElement("Open", bar.Open.ToString()),
                    new XElement("High", bar.High.ToString()),
                    new XElement("Low", bar.Low.ToString()),
                    new XElement("Close", bar.Close.ToString()),
                    new XElement("Volume", bar.Volume.ToString()),
                    new XElement("DateTime", bar.DateTime.ToString()),
                    new XElement("End DateTime", bar.EndDateTime.ToString())
                    );
                barData.Save(fileFullPath);
            }
        }

        public void PrependData(Instrument instrument, Resolution resolution, IEnumerable<Bar> barList)
        {

        }

        public void AppendData(Instrument instrument, Resolution resolution, IEnumerable<Bar> barList)
        {

        }

        private string getFullPath(Instrument instrument, Resolution resolution)
        {
            string directoryName = $"{root}{instrument.Type.ToString()}\\";
            if (instrument.Type == InstrumentType.Forex)
                directoryName += $"{instrument.Name.Substring(0, 3)}{instrument.Name.Substring(4, 3)}";
            else
                directoryName += instrument.Name;

            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);

            return $"{directoryName}\\{resolution.TimeFrame.ToString()}_{resolution.Size}.xml";
        }
    }

}
