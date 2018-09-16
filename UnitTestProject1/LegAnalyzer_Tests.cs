using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trading.Analyzers.LegAnalyzer;
using Trading.Common;

namespace UnitTestProject1
{
    [TestClass]
    public class LegAnalyzer_Tests
    {
        [TestMethod]
        public void AddBarList__Should_Create_UpLeg()
        {
            LegAnalyzer la = new LegAnalyzer(new Resolution(TimeFrame.Daily, 1));

            List<Bar> barList = new List<Bar>();
            barList.Add(Helper.GetUpBar());
            barList.Add(Helper.GetUpBar(barList.Last(), barList.Last().DateTime.AddDays(1)));
            barList.Add(Helper.GetUpBar(barList.Last(), barList.Last().DateTime.AddDays(1)));

            la.AddBarList(barList);

            Assert.AreEqual(la.LegsCount, 1);
            Assert.AreEqual(la.LastLeg.Direction, LegDirection.Up);
            Assert.AreEqual(la.LastLeg.BarCount, 3);
        }

        [TestMethod]
        public void AddBar__Add_UpBar_To_UpLeg()
        {
            LegAnalyzer la = new LegAnalyzer(new Resolution(TimeFrame.Daily, 1));

            List<Bar> barList = new List<Bar>();
            barList.Add(Helper.GetUpBar());
            barList.Add(Helper.GetUpBar(barList.Last(), barList.Last().DateTime.AddDays(1)));
            barList.Add(Helper.GetUpBar(barList.Last(), barList.Last().DateTime.AddDays(1)));
            la.AddBarList(barList);

            la.AddBar(Helper.GetUpBar(la.LastBar, la.LastBar.DateTime.AddDays(1)));

            Assert.AreEqual(la.LastLeg.BarCount, 4);
        }

        //[TestMethod]
        //public void UpdateLastBar_Close_Changed()
        //{
        //    LegAnalyzer la = new LegAnalyzer(new Resolution(TimeFrame.Daily, 1));

        //    List<Bar> barList = new List<Bar>();
        //    barList.Add(Helper.GetUpBar());
        //    barList.Add(Helper.GetUpBar(barList.Last(), barList.Last().DateTime.AddDays(1)));
        //    Bar b = barList.Last();
        //    FxBar b = (FxBar)c.LegAnalyzer.LastBar;
        //    if (e.Item4 > b.DateTime)
        //    {
        //        getNextBarDateTime(b, c.Resolution);
        //        FxBar newBar = new FxBar
        //        {
        //            Open = e.Item2,
        //            AskOpen = e.Item3,
        //            High = e.Item2,
        //            AskHigh = e.Item3,
        //            Low = e.Item2,
        //            AskLow = e.Item3,
        //            Close = e.Item2,
        //            AskClose = e.Item3,
        //            DateTime = getNextBarDateTime(b, c.Resolution)
        //        };
        //        c.LegAnalyzer.AddBar(newBar);
        //        la.UpdateLastBar(new FxBar())
        //}
    }
}
