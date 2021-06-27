using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests;
using Trading.Common;
using Trading.DataBases.XmlDataBase;

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
            var rbs = tb.ReadLocalDataAsync(instrument, new Resolution(TimeFrame.Hourly, 1),
                new DateTime(2018, 11, 22), new DateTime(2018, 11, 22, 20, 0, 0));

            //Assert.AreEqual(barList.Count, rbs.Count());

        }

        [TestMethod]
        public async void WriteData_Stock()
        {
            XmlDataBase tb = new XmlDataBase();

            List<Bar> barList = new List<Bar>();
            barList.Add(new Bar(10, 20, 5, 12, 0, DateTime.Now));
            barList.Add(new Bar(9, 25, 10, 17, 0, DateTime.Now));
            barList.Add(new Bar(8, 27, 8, 17, 0, DateTime.Now));

            var instrument = new Instrument { Type = InstrumentType.Stock, Name = "TSLA" };
            var resolution = new Resolution(TimeFrame.Hourly, 1);
            var b = await tb.WriteLocalDataAsync(instrument, new Resolution(TimeFrame.Hourly, 1), barList);
            var rbs = await tb.ReadLocalDataAsync(instrument, resolution, barList.First().DateTime, barList.Last().DateTime);

            Assert.AreEqual(barList.Count, rbs.Count());

        }

        [TestMethod]
        public async void WriteData_Forex()
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
            var b = await tb.WriteLocalDataAsync(instrument, new Resolution(TimeFrame.Hourly, 1), barList);
            var rbs = await tb.ReadLocalDataAsync(instrument, new Resolution(TimeFrame.Hourly, 1), barList.First().DateTime, barList.Last().DateTime);

            Assert.AreEqual(barList.Count, rbs.Count());

        }

        [TestMethod]
        public async void PrependData()
        {
            XmlDataBase tb = new XmlDataBase();

            List<Bar> barList = new List<Bar>();
            var firstDt = new DateTime(2019, 1, 1, 5, 0, 0);
            barList.Add(new Bar(10, 20, 5, 12, 0, firstDt));
            barList.Add(new Bar(15, 25, 10, 17, 0, firstDt.AddMinutes(5)));
            barList.Add(new Bar(15, 27, 8, 17, 0, firstDt.AddMinutes(10)));

            var instrument = new Instrument { Type = InstrumentType.Stock, Name = "GLD" };
            var resolution = new Resolution(TimeFrame.Hourly, 1);
            var b = await tb.WriteLocalDataAsync(instrument, resolution, barList);

            barList.Clear();
            barList.Add(new Bar(1, 20, 5, 12, 0, firstDt.AddMinutes(-60)));
            barList.Add(new Bar(0, 25, 10, 17, 0, firstDt.AddMinutes(-55)));
            barList.Add(new Bar(0, 27, 8, 17, 0, firstDt.AddMinutes(-50)));
            _ = tb.PrependLocalData(instrument, resolution, barList);

            var rbs = await tb.ReadLocalDataAsync(instrument, resolution, firstDt.AddMinutes(-60), firstDt.AddMinutes(15));

            Assert.AreEqual(6, rbs.Count());

        }

        [TestMethod]
        public async void AppendData_Update_Last_Element()
        {
            XmlDataBase tb = new XmlDataBase();

            List<Bar> barList = new List<Bar>();
            DateTime dt = new DateTime(2018, 1, 1, 0, 0, 0);
            barList.Add(new Bar(10, 20, 5, 12, 0, dt));
            barList.Add(new Bar(15, 25, 10, 17, 0, dt.AddDays(1)));
            barList.Add(new Bar(15, 27, 8, 17, 0, dt.AddDays(2)));

            var instrument = new Instrument { Type = InstrumentType.Stock, Name = "GLD" };
            var resolution = new Resolution(TimeFrame.Hourly, 1);
            var ba = await tb.WriteLocalDataAsync(instrument, resolution, barList);

            barList.Clear();
            barList.Add(new Bar(1, 20, 5, 12, 0, dt.AddDays(2)));
            barList.Add(new Bar(0, 25, 10, 17, 0, dt.AddDays(3)));
            barList.Add(new Bar(0, 27, 8, 17, 0, dt.AddDays(4)));
            _ = tb.AppendLocalData(instrument, resolution, barList);

            var rbs = await tb.ReadLocalDataAsync(instrument, resolution, dt, dt.AddDays(4));

            Assert.AreEqual(5, rbs.Count());
            Assert.AreEqual(dt.AddDays(4), rbs.Last().DateTime);
        }

        [TestMethod]
        public async void AppendData_Last_Element_Intact()
        {
            XmlDataBase tb = new XmlDataBase();

            List<Bar> barList = new List<Bar>();
            DateTime dt = new DateTime(2018, 1, 1, 0, 0, 0);
            barList.Add(new Bar(10, 20, 5, 12, 0, dt));
            barList.Add(new Bar(15, 25, 10, 17, 0, dt.AddDays(1)));
            barList.Add(new Bar(15, 27, 8, 17, 0, dt.AddDays(2)));

            var instrument = new Instrument { Type = InstrumentType.Stock, Name = "GLD" };
            var resolution = new Resolution(TimeFrame.Hourly, 1);
            var b = await tb.WriteLocalDataAsync(instrument, resolution, barList);

            barList.Clear();
            barList.Add(new Bar(1, 20, 5, 12, 0, dt.AddDays(3)));
            barList.Add(new Bar(0, 25, 10, 17, 0, dt.AddDays(4)));
            barList.Add(new Bar(0, 27, 8, 17, 0, dt.AddDays(5)));
            _ = tb.AppendLocalData(instrument, resolution, barList);

            var rbs = await tb.ReadLocalDataAsync(instrument, resolution, dt, dt.AddDays(5));

            Assert.AreEqual(6, rbs.Count());
            Assert.AreEqual(dt.AddDays(3), rbs.ElementAt(3).DateTime);
        }



    }

}
