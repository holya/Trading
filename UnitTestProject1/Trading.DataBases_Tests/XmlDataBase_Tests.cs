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
        public void ReadData()
        {
            XmlDataBase tb = new XmlDataBase();

            var instrument = new Instrument { Type = InstrumentType.Forex, Name = "AUD/CAD" };
            var rbs = tb.ReadLocalData(instrument, new Resolution(TimeFrame.Hourly, 1),
                new DateTime(2018, 11, 22), new DateTime(2018, 11, 22, 20, 0, 0));

            //Assert.AreEqual(barList.Count, rbs.Count());

        }

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
            tb.WriteLocalData(instrument, new Resolution(TimeFrame.Hourly, 1), barList);
            var rbs = tb.ReadLocalData(instrument, resolution, DateTime.Now, DateTime.Now);

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
            tb.WriteLocalData(instrument, new Resolution(TimeFrame.Hourly, 1), barList);
            var rbs = tb.ReadLocalData(instrument, new Resolution(TimeFrame.Hourly, 1), DateTime.Now, DateTime.Now);

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
            tb.WriteLocalData(instrument, resolution, barList);

            barList.Clear();
            barList.Add(new Bar(1, 20, 5, 12, 0, DateTime.Now, DateTime.Now));
            barList.Add(new Bar(0, 25, 10, 17, 0, DateTime.Now, DateTime.Now));
            barList.Add(new Bar(0, 27, 8, 17, 0, DateTime.Now, DateTime.Now));
            tb.PrependLocalData(instrument, resolution, barList);

            var rbs = tb.ReadLocalData(instrument, resolution, DateTime.Now, DateTime.Now);

            Assert.AreEqual(6, rbs.Count());

        }

        [TestMethod]
        public void AppendData_Update_Last_Element()
        {
            XmlDataBase tb = new XmlDataBase();

            List<Bar> barList = new List<Bar>();
            DateTime dt = new DateTime(2018, 1, 1, 0, 0, 0);
            barList.Add(new Bar(10, 20, 5, 12, 0, dt, dt));
            barList.Add(new Bar(15, 25, 10, 17, 0, dt.AddDays(1), dt));
            barList.Add(new Bar(15, 27, 8, 17, 0, dt.AddDays(2), dt));

            var instrument = new Instrument { Type = InstrumentType.Stock, Name = "GLD" };
            var resolution = new Resolution(TimeFrame.Hourly, 1);
            tb.WriteLocalData(instrument, resolution, barList);

            barList.Clear();
            barList.Add(new Bar(1, 20, 5, 12, 0, dt.AddDays(2), dt));
            barList.Add(new Bar(0, 25, 10, 17, 0, dt.AddDays(3), dt));
            barList.Add(new Bar(0, 27, 8, 17, 0, dt.AddDays(4), dt));
            tb.AppendLocalData(instrument, resolution, barList);

            var rbs = tb.ReadLocalData(instrument, resolution, dt, dt.AddDays(4));

            Assert.AreEqual(5, rbs.Count());
            Assert.AreEqual(dt.AddDays(4), rbs.Last().DateTime);
        }

        [TestMethod]
        public void AppendData_Last_Element_Intact()
        {
            XmlDataBase tb = new XmlDataBase();

            List<Bar> barList = new List<Bar>();
            DateTime dt = new DateTime(2018, 1, 1, 0, 0, 0);
            barList.Add(new Bar(10, 20, 5, 12, 0, dt, dt));
            barList.Add(new Bar(15, 25, 10, 17, 0, dt.AddDays(1), dt));
            barList.Add(new Bar(15, 27, 8, 17, 0, dt.AddDays(2), dt));

            var instrument = new Instrument { Type = InstrumentType.Stock, Name = "GLD" };
            var resolution = new Resolution(TimeFrame.Hourly, 1);
            tb.WriteLocalData(instrument, resolution, barList);

            barList.Clear();
            barList.Add(new Bar(1, 20, 5, 12, 0, dt.AddDays(3), dt));
            barList.Add(new Bar(0, 25, 10, 17, 0, dt.AddDays(4), dt));
            barList.Add(new Bar(0, 27, 8, 17, 0, dt.AddDays(5), dt));
            tb.AppendLocalData(instrument, resolution, barList);

            var rbs = tb.ReadLocalData(instrument, resolution, dt, dt.AddDays(5));

            Assert.AreEqual(6, rbs.Count());
            Assert.AreEqual(dt.AddDays(3), rbs.ElementAt(3).DateTime);
        }



    }

}
