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
        public void WriteData__Observe_Valid_Data_Entry()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void WriteData()
        {
            TextDataBase tb = new TextDataBase();

            List<Bar> barList = new List<Bar>();
            barList.Add(Helper.GetUpBar());
            barList.Add(Helper.GetUpBar(barList.Last(), barList.Last().DateTime.AddDays(1)));
            barList.Add(Helper.GetUpBar(barList.Last(), barList.Last().DateTime.AddDays(1)));
            barList.Add(Helper.GetUpBar(barList.Last(), barList.Last().DateTime.AddDays(1)));


            tb.WriteData("EUR/CAD", new Resolution(TimeFrame.Hourly, 4), barList);
            var rbs = tb.ReadData("EUR/CAD", new Resolution(TimeFrame.Daily, 1));

            Assert.AreEqual(barList.Count, rbs.Count());
        }
    }
}
