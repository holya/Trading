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
            Assert.AreEqual(3, la.LastLeg.BarCount);
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

            Assert.AreEqual(4, la.LastLeg.BarCount);
        }

        [TestMethod]
        public void UpdateLastBar_Update_Close()
        {
            LegAnalyzer la = new LegAnalyzer(new Resolution(TimeFrame.Daily, 1));

            la.AddBar(Helper.GetUpBar());
            la.AddBar(Helper.GetUpBar(la.LastBar, la.LastBar.DateTime.AddDays(1)));

            FxBar b = (FxBar)la.LastBar;

            FxBar nb = new FxBar
            {
                Open = b.Open, AskOpen = b.AskOpen, High = b.High + 5, AskHigh = b.AskHigh + 5, Low = b.Low, AskLow = b.AskLow, Close = b.Close + 3, AskClose = b.AskClose + 3, DateTime = b.DateTime, Volume = 100
            };
            la.UpdateLastBar(nb);

            Assert.AreEqual(la.LastBar.Direction, BarDirection.Up);
            Assert.AreEqual(la.LastBar.High, 115);
            Assert.AreEqual(((FxBar)la.LastBar).AskHigh, 116);
            Assert.AreEqual(la.LastBar.Close, 73);
            Assert.AreEqual(((FxBar)la.LastBar).AskClose, 74);
            Assert.AreEqual(la.LastBar.Volume, 200);


        }
    }
}
