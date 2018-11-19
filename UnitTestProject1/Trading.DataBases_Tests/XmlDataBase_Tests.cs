using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trading.Common;
using Trading.DataBases.XmlDataBase;
using UnitTestProject1;

namespace Trading.DataBases_Tests
{
    [TestClass]
    public class XmlDataBase_Tests
    {
        [TestMethod]
        public void WriteData_Stock()
        {
            XmlDataBase tb = new XmlDataBase();

            List<Bar> barList = new List<Bar>();
            barList.Add(new Bar(10, 20, 5, 12, 0, DateTime.Now, DateTime.Now));
            barList.Add(new Bar(9, 25, 10, 17, 0, DateTime.Now, DateTime.Now));
            barList.Add(new Bar(8, 27, 8, 17, 0, DateTime.Now, DateTime.Now));

            var instrument = new Instrument { Type = InstrumentType.Stock, Name = "TSLA" };
            var resolution = new Resolution(TimeFrame.Hourly, 1);
            tb.WriteData(instrument, new Resolution(TimeFrame.Hourly, 1), barList);
            var rbs = tb.ReadData(instrument, resolution);

            Assert.AreEqual(barList.Count, rbs.Count());

        }

        [TestMethod]
        public void WriteData_Forex()
        {
            XmlDataBase tb = new XmlDataBase();

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
        public void PrependData()
        {
            XmlDataBase tb = new XmlDataBase();

            List<Bar> barList = new List<Bar>();
            barList.Add(new Bar(10, 20, 5, 12, 0, DateTime.Now, DateTime.Now));
            barList.Add(new Bar(15, 25, 10, 17, 0, DateTime.Now, DateTime.Now));
            barList.Add(new Bar(15, 27, 8, 17, 0, DateTime.Now, DateTime.Now));

            var instrument = new Instrument { Type = InstrumentType.Stock, Name = "GLD" };
            var resolution = new Resolution(TimeFrame.Hourly, 1);
            tb.WriteData(instrument, resolution, barList);

            barList.Clear();
            barList.Add(new Bar(1, 20, 5, 12, 0, DateTime.Now, DateTime.Now));
            barList.Add(new Bar(0, 25, 10, 17, 0, DateTime.Now, DateTime.Now));
            barList.Add(new Bar(0, 27, 8, 17, 0, DateTime.Now, DateTime.Now));
            tb.PrependData(instrument, resolution, barList);

            var rbs = tb.ReadData(instrument, resolution);

            Assert.AreEqual(6, rbs.Count());

        }


    }

}
