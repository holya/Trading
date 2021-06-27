using System;
using System.Collections.Generic;
using System.IO;
using Trading.Common;
using Trading.DataBases.Common;

namespace Trading.DataBases.TextFileDataBase
{
    public class TextDataBase : IDataBase
    {
        public string root = "C:\\DataBase\\TextFiles\\";

        public TextDataBase() { }
        public TextDataBase(string rootPath)
        {
            root = rootPath;
        }

        public IEnumerable<Bar> ReadLocalDataAsync(Instrument instrument, Resolution resolution, DateTime fromDate, DateTime toDate)
        {
            List<Bar> barList = new List<Bar>();

            if (!File.Exists(getFullPath(instrument, resolution)))
                return barList;

            using (StreamReader sr = new StreamReader(getFullPath(instrument, resolution)))
            {
                string line;
                if (instrument.Type == InstrumentType.Forex)
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] str = line.Split(new char[] { ',' });
                        FxBar b = new FxBar(Convert.ToDouble(str[0]), Convert.ToDouble(str[1]), Convert.ToDouble(str[2]), Convert.ToDouble(str[3]), Convert.ToDouble(str[4]), Convert.ToDouble(str[5]), Convert.ToDouble(str[6]), Convert.ToDouble(str[7]), Convert.ToDouble(str[8]), Convert.ToDateTime(str[9]), Convert.ToDateTime(str[10]));

                        barList.Add(b);
                    }
                }
                else
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] str = line.Split(new char[] { ',' });
                        Bar b = new Bar(Convert.ToDouble(str[0]), Convert.ToDouble(str[1]), Convert.ToDouble(str[2]), Convert.ToDouble(str[3]), Convert.ToDouble(str[4]), Convert.ToDateTime(str[5]), Convert.ToDateTime(str[6]));

                        barList.Add(b);
                    }
                }
            }

            return barList;
        }

        public void WriteLocalDataAsync(Instrument instrument, Resolution resolution, IEnumerable<Bar> barList)
        {
            string fileFullPath = getFullPath(instrument, resolution);

            using (StreamWriter sw = new StreamWriter(fileFullPath))
            {
                foreach (var bar in barList)
                    sw.WriteLine(bar.ToString());
            }
        }

        public void PrependLocalData(Instrument instrument, Resolution resolution, IEnumerable<Bar> barList)
        {
            string fileFullPath = getFullPath(instrument, resolution);

            var localList = ReadLocalDataAsync(instrument, resolution, DateTime.Now, DateTime.Now);

            using (StreamWriter sw = new StreamWriter(fileFullPath))
            {

                foreach (var bar in barList)
                    sw.WriteLine(bar.ToString());

                foreach (var bar in localList)
                    sw.WriteLine(bar.ToString());
            }
        }

        public void AppendLocalData(Instrument instrument, Resolution resolution, IEnumerable<Bar> barList)
        {
            string path = getFullPath(instrument, resolution);

            using (StreamWriter sw = File.AppendText(path))
            {
                foreach (var bar in barList)
                    sw.WriteLine(bar.ToString());
            }
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

            return $"{directoryName}\\{resolution.TimeFrame.ToString()}_{resolution.Size}.txt";
        }
    }
}
