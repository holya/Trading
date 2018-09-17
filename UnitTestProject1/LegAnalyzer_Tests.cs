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

            Assert.AreEqual(1, la.LegsCount);
            Assert.AreEqual(LegDirection.Up, la.LastLeg.Direction);
            Assert.AreEqual(3, la.LastLeg.BarCount);
        }

        [TestMethod]
        public void AddBarList__Should_Create_One_DownLeg_One_UpLeg()
        {
            LegAnalyzer la = new LegAnalyzer(new Resolution(TimeFrame.Daily, 1));

            List<Bar> barList = new List<Bar>();
            barList.Add(Helper.GetDownBar());
            barList.Add(Helper.GetDownBar(barList.Last(), barList.Last().DateTime.AddDays(1)));
            barList.Add(Helper.GetBalanceBar(barList.Last(), barList.Last().DateTime.AddDays(1)));

            barList.Add(Helper.GetUpBar(barList.Last(), barList.Last().DateTime.AddDays(1)));

            la.AddBarList(barList);

            Assert.AreEqual(2, la.LegsCount);
            Assert.AreEqual(LegDirection.Up, la.LastLeg.Direction);
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

            Assert.AreEqual(BarDirection.Up, la.LastBar.Direction);
            Assert.AreEqual(115, la.LastBar.High);
            Assert.AreEqual(116, ((FxBar)la.LastBar).AskHigh);
            Assert.AreEqual(73, la.LastBar.Close);
            Assert.AreEqual(74, ((FxBar)la.LastBar).AskClose);
            Assert.AreEqual(200, la.LastBar.Volume);
        }

        [TestMethod]
        public void UpdateLastBar__Turn_LastBar_To_DownBar_And_Create_New_DownLeg()
        {
            LegAnalyzer la = new LegAnalyzer(new Resolution(TimeFrame.Daily, 1));

            List<Bar> barList = new List<Bar>();
            barList.Add(Helper.GetUpBar());
            barList.Add(Helper.GetUpBar(barList.Last(), barList.Last().DateTime.AddDays(1)));
            barList.Add(Helper.GetUpBar(barList.Last(), barList.Last().DateTime.AddDays(1)));

            la.AddBarList(barList);

            FxBar b = (FxBar)la.LastLeg.BarList[1];
            FxBar nb = new FxBar
            {
                Open = b.Open,
                AskOpen = b.AskOpen,
                High = b.High - 5,
                AskHigh = b.AskHigh - 5,
                Low = b.Low - 5,
                AskLow = b.AskLow - 5,
                Close = b.Open - 3,
                AskClose = b.AskOpen - 3,
                DateTime = b.DateTime,
                Volume = 100
            };
            la.UpdateLastBar(nb);

            Assert.AreEqual(2, la.LegsCount);
            Assert.AreEqual(LegDirection.Down, la.LastLeg.Direction);

        }
    }
}
