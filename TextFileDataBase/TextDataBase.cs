using System;
using System.Collections.Generic;
using System.IO;
using Trading.Common;
using Trading.DataBases.Interfaces;

namespace Trading.Databases.TextFileDataBase
{
    public class TextDataBase : IDataBase
    {
        public string root = "C:\\DataBase\\TextFiles\\";

        public TextDataBase() { }
        public TextDataBase(string rootPath)
        {
            root = rootPath;
        }

        public IEnumerable<Bar> ReadData(Instrument instrument, Resolution resolution)
        {
            List<Bar> barList = new List<Bar>();

            string directoryName = $"{getSymbolFolderName(instrument)}";
            if (!Directory.Exists(directoryName))
                return barList;

            string fn = getFileName(resolution);
            string fileFullPath = $"{directoryName}\\{fn}.txt";
            if (!File.Exists(fileFullPath))
                return barList;

            using (StreamReader sr = new StreamReader(fileFullPath))
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

        public void WriteData(Instrument instrument, Resolution resolution, IEnumerable<Bar> barList)
        {
            string fileFullPath = getFullPath(instrument, resolution);

            using (StreamWriter sw = new StreamWriter(fileFullPath))
            {
                foreach (var bar in barList)
                    sw.WriteLine(bar.ToString());

                sw.Close();
            }
        }

        public void PrependData(Instrument instrument, Resolution resolution, IEnumerable<Bar> barList)
        {
            string fileFullPath = getFullPath(instrument, resolution);

            var localList = ReadData(instrument, resolution);

            using (StreamWriter sw = new StreamWriter(fileFullPath))
            {

                foreach (var bar in barList)
                    sw.WriteLine(bar.ToString());

                foreach (var bar in localList)
                    sw.WriteLine(bar.ToString());
            }
        }

        public void AppendData(Instrument instrument, Resolution resolution, IEnumerable<Bar> barList)
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
            string directoryName = $"{getSymbolFolderName(instrument)}";
            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);

            string fileFullPath = $"{directoryName}\\{getFileName(resolution)}.txt";
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
