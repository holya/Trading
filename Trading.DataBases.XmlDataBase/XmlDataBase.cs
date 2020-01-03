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

        public IEnumerable<Bar> ReadLocalData(Instrument instrument, Resolution resolution, DateTime fromDate, DateTime toDate)
        {
            List<Bar> barList = new List<Bar>();

            if (!File.Exists(getFullPath(instrument, resolution)))
                return barList;

            XElement readData = XElement.Load(getFullPath(instrument, resolution));

            IEnumerable<XElement> barData = from elements in readData.Descendants()
                                            where (Convert.ToDateTime(elements.Attribute("DateTime").Value) >= fromDate 
                                            && Convert.ToDateTime(elements.Attribute("EndDateTime").Value) <= toDate)
                                            select elements;

            if (instrument.Type == InstrumentType.Forex)
            {
                foreach (var elem in barData)
                {
                    barList.Add(new FxBar
                    {
                        Open = Convert.ToDouble(elem.Attribute("Open").Value),
                        AskOpen = Convert.ToDouble(elem.Attribute("AskOpen").Value),
                        High = Convert.ToDouble(elem.Attribute("High").Value),
                        AskHigh = Convert.ToDouble(elem.Attribute("AskHigh").Value),
                        Low = Convert.ToDouble(elem.Attribute("Low").Value),
                        AskLow = Convert.ToDouble(elem.Attribute("AskLow").Value),
                        Close = Convert.ToDouble(elem.Attribute("Close").Value),
                        AskClose = Convert.ToDouble(elem.Attribute("AskClose").Value),
                        Volume = Convert.ToDouble(elem.Attribute("Volume").Value),
                        DateTime = Convert.ToDateTime(elem.Attribute("DateTime").Value),
                        EndDateTime = Convert.ToDateTime(elem.Attribute("EndDateTime").Value)
                    });
                }
            }
            else
            {
                foreach (var elem in barData)
                {
                    barList.Add(new Bar
                    {
                        Open = Convert.ToDouble(elem.Attribute("Open").Value),
                        High = Convert.ToDouble(elem.Attribute("High").Value),
                        Low = Convert.ToDouble(elem.Attribute("Low").Value),
                        Close = Convert.ToDouble(elem.Attribute("Close").Value),
                        Volume = Convert.ToDouble(elem.Attribute("Volume").Value),
                        DateTime = Convert.ToDateTime(elem.Attribute("DateTime").Value),
                        EndDateTime = Convert.ToDateTime(elem.Attribute("EndDateTime").Value)
                    });
                }
            }

            return barList;
        }

        public void WriteLocalData(Instrument instrument, Resolution resolution, IEnumerable<Bar> barList)
        {
            XDocument doc = new XDocument(new XElement("root"));

            if (instrument.Type == InstrumentType.Stock)
                doc.Root.Add(createXElementListFrom_BarList(barList));
            else
                doc.Root.Add(createXElementListFrom_FxBarList(barList));

            doc.Save(getFullPath(instrument, resolution));
        }

        private static IEnumerable<XElement> createXElementListFrom_BarList(IEnumerable<Bar> barList)
        {
            return barList.Select(bar => new XElement("bar",
                                            new XAttribute("Open", bar.Open),
                                            new XAttribute("High", bar.High),
                                            new XAttribute("Low", bar.Low),
                                            new XAttribute("Close", bar.Close),
                                            new XAttribute("Volume", bar.Volume),
                                            new XAttribute("DateTime", bar.DateTime),
                                            new XAttribute("EndDateTime", bar.EndDateTime)
                                            ));
        }

        private IEnumerable<XElement> createXElementListFrom_FxBarList(IEnumerable<Bar> barList)
        {
            return barList.Select(bar => new XElement("bar",
                                new XAttribute("Open", ((FxBar)bar).Open),
                                new XAttribute("AskOpen", ((FxBar)bar).AskOpen),
                                new XAttribute("High", ((FxBar)bar).High),
                                new XAttribute("AskHigh", ((FxBar)bar).AskHigh),
                                new XAttribute("Low", ((FxBar)bar).Low),
                                new XAttribute("AskLow", ((FxBar)bar).AskLow),
                                new XAttribute("Close", ((FxBar)bar).Close),
                                new XAttribute("AskClose", ((FxBar)bar).AskClose),
                                new XAttribute("Volume", bar.Volume),
                                new XAttribute("DateTime", bar.DateTime),
                                new XAttribute("EndDateTime", bar.EndDateTime)
                                ));
        }

        public void PrependLocalData(Instrument instrument, Resolution resolution, IEnumerable<Bar> barList)
        {
            string fileFullPath = getFullPath(instrument, resolution);
            XDocument doc = XDocument.Load(fileFullPath);

            if (instrument.Type == InstrumentType.Stock)
            {
                doc.Root.AddFirst(createXElementListFrom_BarList(barList));
            }
            else
            {
                doc.Root.AddFirst(createXElementListFrom_FxBarList(barList));
            }

            doc.Save(fileFullPath);
        }

        public void AppendLocalData(Instrument instrument, Resolution resolution, IEnumerable<Bar> barList)
        {
            string fileFullPath = getFullPath(instrument, resolution);
            XDocument doc = XDocument.Load(fileFullPath);

            var lastElement = doc.Root.Elements().Last();
            if (barList.First().DateTime == DateTime.Parse(lastElement.Attribute("DateTime").Value))
                lastElement.Remove();

            if (instrument.Type == InstrumentType.Stock)
            {
                doc.Root.Add(createXElementListFrom_BarList(barList));
            }
            else
            {
                doc.Root.Add(createXElementListFrom_FxBarList(barList));
            }

            doc.Save(fileFullPath);
        }

        private string getFullPath(Instrument instrument, Resolution resolution)
        {
            string directoryName = $"{root}{instrument.Type.ToString()}\\{instrument.Name}";
            //if (instrument.Type == InstrumentType.Forex)
            //    directoryName += $"{instrument.Name.Substring(0, 3)}{instrument.Name.Substring(4, 3)}";
            //else
            //    directoryName += instrument.Name;

            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);

            return $"{directoryName}\\{resolution.TimeFrame.ToString()}_{resolution.Size}.xml";
        }
    }

}
