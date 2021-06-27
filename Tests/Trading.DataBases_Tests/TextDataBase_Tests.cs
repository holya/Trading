//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Tests;
//using Trading.Common;
//using Trading.DataBases.TextFileDataBase;

//namespace Trading.DataBases_Tests
//{
//    [TestClass]
//    public class TextDataBase_Tests
//    {
//        [TestMethod]
//        public void WriteData_TextDataBase_Forex()
//        {
//            TextDataBase tb = new TextDataBase();

//            List<Bar> barList = new List<Bar>();
//            barList.Add(Helper.GetUpBar());
//            barList.Add(Helper.GetUpBar(barList.Last(), barList.Last().DateTime.AddDays(1)));
//            barList.Add(Helper.GetUpBar(barList.Last(), barList.Last().DateTime.AddDays(1)));
//            barList.Add(Helper.GetUpBar(barList.Last(), barList.Last().DateTime.AddDays(1)));
//            barList.Add(Helper.GetUpBar(barList.Last(), barList.Last().DateTime.AddDays(1)));
//            barList.Add(Helper.GetUpBar(barList.Last(), barList.Last().DateTime.AddDays(1)));

//            var instrument = new Instrument { Type = InstrumentType.Forex, Name = "USD/CAD" };
//            tb.WriteLocalDataAsync(instrument, new Resolution(TimeFrame.Hourly, 1), barList);
//            var rbs = tb.ReadLocalDataAsync(instrument, new Resolution(TimeFrame.Hourly, 1), DateTime.Now, DateTime.Now);

//            Assert.AreEqual(barList.Count, rbs.Count());
//        }

//        [TestMethod]
//        public void WriteData_TextDataBase_Stock()
//        {
//            TextDataBase tb = new TextDataBase();

//            List<Bar> barList = new List<Bar>();
//            barList.Add(new Bar(10, 20, 5, 12, 0, DateTime.Now, DateTime.Now));
//            barList.Add(new Bar(15, 25, 10, 17, 0, DateTime.Now, DateTime.Now));
//            barList.Add(new Bar(15, 27, 8, 17, 0, DateTime.Now, DateTime.Now));

//            var instrument = new Instrument { Type = InstrumentType.Stock, Name = "MS" };
//            tb.WriteLocalDataAsync(instrument, new Resolution(TimeFrame.Hourly, 1), barList);
//            var rbs = tb.ReadLocalDataAsync(instrument, new Resolution(TimeFrame.Hourly, 1), DateTime.Now, DateTime.Now);

//            Assert.AreEqual(barList.Count, rbs.Count());
//        }

//        [TestMethod]
//        public void PrependData__adds_data_to_beginning_of_file()
//        {
//            TextDataBase tb = new TextDataBase();

//            List<Bar> barList = new List<Bar>();
//            barList.Add(new Bar(10, 20, 5, 12, 0, DateTime.Now, DateTime.Now));
//            barList.Add(new Bar(15, 25, 10, 17, 0, DateTime.Now, DateTime.Now));
//            barList.Add(new Bar(15, 27, 8, 17, 0, DateTime.Now, DateTime.Now));

//            var instrument = new Instrument { Type = InstrumentType.Stock, Name = "TSLA" };
//            var resolution = new Resolution(TimeFrame.Hourly, 1);
//            tb.WriteLocalDataAsync(instrument, resolution, barList);

//            barList.Clear();
//            barList.Add(new Bar(1, 20, 5, 12, 0, DateTime.Now, DateTime.Now));
//            barList.Add(new Bar(0, 25, 10, 17, 0, DateTime.Now, DateTime.Now));
//            barList.Add(new Bar(0, 27, 8, 17, 0, DateTime.Now, DateTime.Now));
//            tb.PrependLocalData(instrument, resolution, barList);

//            var rbs = tb.ReadLocalDataAsync(instrument, resolution, DateTime.Now, DateTime.Now);

//            Assert.AreEqual(6, rbs.Count());
//        }

//        [TestMethod]
//        public void AppendData__adds_data_to_end_of_file()
//        {
//            TextDataBase tb = new TextDataBase();

//            List<Bar> barList = new List<Bar>();
//            barList.Add(new Bar(10, 20, 5, 12, 0, DateTime.Now, DateTime.Now));
//            barList.Add(new Bar(15, 25, 10, 17, 0, DateTime.Now, DateTime.Now));
//            barList.Add(new Bar(15, 27, 8, 17, 0, DateTime.Now, DateTime.Now));

//            var instrument = new Instrument { Type = InstrumentType.Stock, Name = "TSLA" };
//            var resolution = new Resolution(TimeFrame.Hourly, 1);
//            tb.WriteLocalDataAsync(instrument, resolution, barList);

//            barList.Clear();
//            barList.Add(new Bar(0, 20, 5, 12, 0, DateTime.Now, DateTime.Now));
//            barList.Add(new Bar(0, 25, 10, 17, 0, DateTime.Now, DateTime.Now));
//            barList.Add(new Bar(0, 27, 8, 17, 0, DateTime.Now, DateTime.Now));
//            tb.AppendLocalData(instrument, resolution, barList);

//            var rbs = tb.ReadLocalDataAsync(instrument, resolution, DateTime.Now, DateTime.Now);

//            Assert.AreEqual(6, rbs.Count());

//        }

//    }
//}
