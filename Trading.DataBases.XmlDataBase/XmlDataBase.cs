using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Xml;
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
            List<Bar> barList = new List<Bar>();
            Bar temp = new Bar();

            if (!File.Exists(getFullPath(instrument, resolution)))
                return barList;

            XElement readData = XElement.Load(getFullPath(instrument, resolution));
            IEnumerable<XElement> barData = from elements in readData.Elements("Bar Data")
                                            select elements;

            foreach (var b in barData)
            {
                temp.Open = Convert.ToDouble(b.Attribute("Open").Value);
                temp.High = Convert.ToDouble(b.Attribute("High").Value);
                temp.Low = Convert.ToDouble(b.Attribute("Low").Value);
                temp.Close = Convert.ToDouble(b.Attribute("Close").Value);
                temp.Volume = Convert.ToDouble(b.Attribute("Volume").Value);
                temp.DateTime = Convert.ToDateTime(b.Attribute("DateTime").Value);
                temp.EndDateTime = Convert.ToDateTime(b.Attribute("End DateTime").Value);
                barList.Add(temp);
            }

            return barList;
        }

        public void WriteData(Instrument instrument, Resolution resolution, IEnumerable<Bar> barList)
        {
            string fileFullPath = getFullPath(instrument, resolution);

            XElement barData;
            
            foreach (var bar in barList)
            {
                barData = new XElement("Bar Data",
                    new XAttribute("Open", bar.Open),
                    new XAttribute("High", bar.High),
                    new XAttribute("Low", bar.Low),
                    new XAttribute("Close", bar.Close),
                    new XAttribute("Volume", bar.Volume),
                    new XAttribute("DateTime", bar.DateTime),
                    new XAttribute("End DateTime", bar.EndDateTime)
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
