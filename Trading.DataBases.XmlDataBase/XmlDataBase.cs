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

            if (!File.Exists(getFullPath(instrument, resolution)))
                return barList;

            XElement readData = XElement.Load(getFullPath(instrument, resolution));
            IEnumerable<XElement> barData = from elements in readData.Descendants()
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

        public void WriteData(Instrument instrument, Resolution resolution, IEnumerable<Bar> barList)
        {
            string fileFullPath = getFullPath(instrument, resolution);

            //XDocument doc = new XDocument(new XElement("root"));
            //XElement root = doc.Elements().First();
            //foreach (var bar in barList)
            //{
            //root.Add(new XElement("bar",
            //    new XAttribute("Open", bar.Open),
            //    new XAttribute("High", bar.High),
            //    new XAttribute("Low", bar.Low),
            //    new XAttribute("Close", bar.Close),
            //    new XAttribute("Volume", bar.Volume),
            //    new XAttribute("DateTime", bar.DateTime),
            //    new XAttribute("EndDateTime", bar.EndDateTime)
            //    ));
            //}
            //doc.Save(fileFullPath);
            //));
            XElement root = new XElement("root");
            XDocument doc = new XDocument(root);

            if (instrument.Type == InstrumentType.Stock)
            {
                root.Add(barList.Select(bar => new XElement("bar",
                                new XAttribute("Open", bar.Open),
                                new XAttribute("High", bar.High),
                                new XAttribute("Low", bar.Low),
                                new XAttribute("Close", bar.Close),
                                new XAttribute("Volume", bar.Volume),
                                new XAttribute("DateTime", bar.DateTime),
                                new XAttribute("EndDateTime", bar.EndDateTime)
                                )));
            }
            else
            {
                root.Add(barList.Select(bar => createXElementFromFxBar(bar)));
            }


            doc.Save(fileFullPath);
        }

        private static XElement createXElementFromBar(Bar bar)
        {
            var e = new XElement("bar",
                                new XAttribute("Open", bar.Open),
                                new XAttribute("High", bar.High),
                                new XAttribute("Low", bar.Low),
                                new XAttribute("Close", bar.Close),
                                new XAttribute("Volume", bar.Volume),
                                new XAttribute("DateTime", bar.DateTime),
                                new XAttribute("EndDateTime", bar.EndDateTime)
                                );
            return e;
        }
        private static XElement createXElementFromFxBar(Bar bar)
        {
            return new XElement("bar",
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
                                );
        }


        public void PrependData(Instrument instrument, Resolution resolution, IEnumerable<Bar> barList)
        {
            string fileFullPath = getFullPath(instrument, resolution);

            XDocument doc = XDocument.Load(fileFullPath);
            if (instrument.Type == InstrumentType.Stock)
            {
                doc.Root.AddFirst(barList.Select(bar => new XElement("bar",
                                        new XAttribute("Open", bar.Open),
                                        new XAttribute("High", bar.High),
                                        new XAttribute("Low", bar.Low),
                                        new XAttribute("Close", bar.Close),
                                        new XAttribute("Volume", bar.Volume),
                                        new XAttribute("DateTime", bar.DateTime),
                                        new XAttribute("EndDateTime", bar.EndDateTime)
                                        )
                ));
            }
            else
            {
                doc.Root.AddFirst(barList.Select(bar => new XElement("bar",
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
                                )));
            }

            doc.Save(fileFullPath);
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
