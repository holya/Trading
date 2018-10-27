using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trading.Common;
using Trading.Databases.TextFileDataBase;
namespace UnitTestProject1
{
    [TestClass]
    public class TextDataBase_Tests
    {
        [TestMethod]
        public void WriteData_Forex()
        {
            TextDataBase tb = new TextDataBase();

            List<Bar> barList = new List<Bar>();
            barList.Add(Helper.GetUpBar());
            barList.Add(Helper.GetUpBar(barList.Last(), barList.Last().DateTime.AddDays(1)));
            barList.Add(Helper.GetUpBar(barList.Last(), barList.Last().DateTime.AddDays(1)));
            barList.Add(Helper.GetUpBar(barList.Last(), barList.Last().DateTime.AddDays(1)));
            barList.Add(Helper.GetUpBar(barList.Last(), barList.Last().DateTime.AddDays(1)));
            barList.Add(Helper.GetUpBar(barList.Last(), barList.Last().DateTime.AddDays(1)));

            var instrument = new Instrument { Type = InstrumentType.Forex, Name = "USD/CAD" };
            tb.WriteData(instrument, new Resolution(TimeFrame.Hourly, 1), barList);
            var rbs = tb.ReadData(instrument, new Resolution(TimeFrame.Hourly, 1));

            Assert.AreEqual(barList.Count, rbs.Count());
        }

        [TestMethod]
        public void WriteData_Stock()
        {
            TextDataBase tb = new TextDataBase();

            List<Bar> barList = new List<Bar>();
            barList.Add(new Bar(10, 20, 5, 12, 0, DateTime.Now, DateTime.Now));
            barList.Add(new Bar(15, 25, 10, 17, 0, DateTime.Now, DateTime.Now));
            barList.Add(new Bar(15, 27, 8, 17, 0, DateTime.Now, DateTime.Now));

            var instrument = new Instrument { Type = InstrumentType.Stock, Name = "MS" };
            tb.WriteData(instrument, new Resolution(TimeFrame.Hourly, 1), barList);
            var rbs = tb.ReadData(instrument, new Resolution(TimeFrame.Hourly, 1));

            Assert.AreEqual(barList.Count, rbs.Count());
        }

    }
}
