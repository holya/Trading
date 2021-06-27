using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using System.Threading.Tasks;
using Trading.Common;
using Trading.DataBases.Common;

namespace Trading.DataBases.XmlDataBase
{
    public class XmlDataBase : IDataBase
    {
        public string root = "C:\\DataBase\\XML\\";

        public async Task<IEnumerable<Bar>> ReadLocalDataAsync(Instrument instrument, Resolution resolution, DateTime fromDate, DateTime toDate)
        {
            return await Task.Factory.StartNew(() =>
                {
                    List<Bar> barList = new List<Bar>();

                    if (!File.Exists(getFullPath(instrument, resolution)))
                        return barList;

                    XElement readData = XElement.Load(getFullPath(instrument, resolution));

                    IEnumerable<XElement> barData = from elements in readData.Descendants()
                                                    where (Convert.ToDateTime(elements.Attribute("DateTime").Value) >= fromDate
                                                    && Convert.ToDateTime(elements.Attribute("DateTime").Value) < toDate)
                                                    select elements;

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
                        });
                    }

                    return barList;
                });
        }

        public async Task<IEnumerable<Bar>> ReadLocalDataAsync(Instrument instrument, Resolution resolution)
        {
            return await Task.Factory.StartNew(( ) =>
            {
                List<Bar> barList = new List<Bar>();

                if (!File.Exists(getFullPath(instrument, resolution)))
                    return barList;

                XElement readData = XElement.Load(getFullPath(instrument, resolution));

                IEnumerable<XElement> barData = from elements in readData.Descendants()
                                                select elements;

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
                    });
                }

                return barList;
            });
        }


        public async Task<bool> WriteLocalDataAsync(Instrument instrument, Resolution resolution, IEnumerable<Bar> barList)
        {
            return await Task.Factory.StartNew(( ) =>
                    {
                        XDocument doc = new XDocument(new XElement("root"));

                        doc.Root.Add(createXElementListFrom_BarList(barList));
                        doc.Save(getFullPath(instrument, resolution));

                        return true;
                    });
        }

        private static IEnumerable<XElement> createXElementListFrom_BarList(IEnumerable<Bar> barList)
        {
            return barList.Select(bar => new XElement("bar",
                                            new XAttribute("Open", bar.Open),
                                            new XAttribute("High", bar.High),
                                            new XAttribute("Low", bar.Low),
                                            new XAttribute("Close", bar.Close),
                                            new XAttribute("Volume", bar.Volume),
                                            new XAttribute("DateTime", bar.DateTime)
                                            ));
        }

        public async Task<bool> PrependLocalData(Instrument instrument, Resolution resolution, IEnumerable<Bar> barList)
        {
            return await Task.Factory.StartNew(( ) =>
                {
                    string fileFullPath = getFullPath(instrument, resolution);
                    XDocument doc = XDocument.Load(fileFullPath);

                    doc.Root.AddFirst(createXElementListFrom_BarList(barList));

                    doc.Save(fileFullPath);

                    return true;
                });
        }

        public async Task<bool> AppendLocalData(Instrument instrument, Resolution resolution, IEnumerable<Bar> barList)
        {
            return await Task.Factory.StartNew(() =>
                {
                    string fileFullPath = getFullPath(instrument, resolution);
                    XDocument doc = XDocument.Load(fileFullPath);

                    doc.Root.Add(createXElementListFrom_BarList(barList));

                    doc.Save(fileFullPath);

                    return true;
                });
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

        public bool FileExists(Instrument instrument, Resolution resolution)
        {
            return File.Exists(getFullPath(instrument, resolution));
        }
    }

}
