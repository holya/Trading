using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trading.Analyzers.LegAnalyzer;
using Trading.Common;
using Trading_UnitTests;

namespace Trading_UnitTests
{
    [TestClass]
    public class LegAnalyzer_Tests
    {
        [TestMethod]
        public void AddBarList__Should_Create_UpLeg()
        {
            LegAnalyzer la = new LegAnalyzer();

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
            LegAnalyzer la = new LegAnalyzer();

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
            LegAnalyzer la = new LegAnalyzer();

            List<Bar> barList = new List<Bar>();
            barList.Add(Helper.GetUpBar());
            barList.Add(Helper.GetUpBar(barList.Last(), barList.Last().DateTime.AddDays(1)));
            barList.Add(Helper.GetUpBar(barList.Last(), barList.Last().DateTime.AddDays(1)));
            la.AddBarList(barList);

            la.AddBar(Helper.GetUpBar(la.LastBar, la.LastBar.DateTime.AddDays(1)));

            Assert.AreEqual(4, la.LastLeg.BarCount);
        }


        [TestMethod]
        public void UpdateLastBar__Update_Close()
        {
            LegAnalyzer la = new LegAnalyzer();

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
            LegAnalyzer la = new LegAnalyzer();

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

        [TestMethod]
        public void AddBar__FirstLeg_FirstBar_TypeChanged()
        {
            LegAnalyzer la = new LegAnalyzer();

            la.AddBar(new Bar(10, 20, 5, 12, 0, DateTime.Now, DateTime.Now));
            la.UpdateLastBar(new Bar(10, 20, 5, 9, 0, DateTime.Now, DateTime.Now));

            Assert.AreEqual(BarDirection.OutsideDown, la.LastBar.Direction);
        }

        [TestMethod]
        public void OutsideUpBar_After_UpLeg_Should_Create_New_UpLeg()
        {
            LegAnalyzer la = new LegAnalyzer();

            la.AddBar(new Bar(10, 20, 5, 12, 0, DateTime.Now, DateTime.Now));
            la.AddBar(new Bar(15, 25, 10, 17, 0, DateTime.Now, DateTime.Now));

            la.AddBar(new Bar(15, 27, 8, 17, 0, DateTime.Now, DateTime.Now));

            Assert.AreEqual(2, la.LegsCount);
            Assert.AreEqual(LegDirection.Up, la.LastLeg.Direction);

            Assert.AreEqual(2, la.RefList.Count);
            Assert.AreEqual(25, la.RefList[0].Price);
            Assert.AreEqual(8, la.RefList[1].Price);
        }

        [TestMethod]
        public void References__DownLeg_then_UpBar()
        {
            LegAnalyzer la = new LegAnalyzer();
            la.AddBar(new Bar(100, 110, 90, 95, 0, DateTime.Now, DateTime.Now));
            la.AddBar(new Bar(99, 109, 89, 94, 0, DateTime.Now, DateTime.Now));

            la.AddBar(new Bar(100, 112, 91, 94, 0, DateTime.Now, DateTime.Now));

            Assert.AreEqual(1, la.RefList.Count);
            Assert.AreEqual(89, la.RefList[0].Price);
        }

        [TestMethod]
        public void References__DownLeg_then_OutsideUpBar()
        {
            LegAnalyzer la = new LegAnalyzer();
            la.AddBar(new Bar(100, 110, 90, 95, 0, DateTime.Now, DateTime.Now));
            la.AddBar(new Bar(99, 109, 89, 94, 0, DateTime.Now, DateTime.Now));

            la.AddBar(new Bar(94, 112, 88, 95, 0, DateTime.Now, DateTime.Now));

            Assert.AreEqual(1, la.RefList.Count);
            Assert.AreEqual(88, la.RefList[0].Price);
        }

        [TestMethod]
        public void References__DownLeg_then_BalanceBar_then_UpBar()
        {
            LegAnalyzer la = new LegAnalyzer();
            la.AddBar(new Bar(100, 110, 90, 95, 0, DateTime.Now, DateTime.Now));
            la.AddBar(new Bar(99, 109, 89, 94, 0, DateTime.Now, DateTime.Now));
            la.AddBar(new Bar(98, 108, 90, 94, 0, DateTime.Now, DateTime.Now));

            la.AddBar(new Bar(97, 112, 91, 95, 0, DateTime.Now, DateTime.Now));

            Assert.AreEqual(1, la.RefList.Count);
            Assert.AreEqual(89, la.RefList[0].Price);
        }

        [TestMethod]
        public void References__DownLeg_then_BalanceBar_then_OutsideUpBar_With_Higher_Low()
        {
            LegAnalyzer la = new LegAnalyzer();
            la.AddBar(new Bar(100, 110, 90, 95, 0, DateTime.Now, DateTime.Now));
            la.AddBar(new Bar(99, 109, 89, 94, 0, DateTime.Now, DateTime.Now));
            la.AddBar(new Bar(96, 106, 94, 96, 0, DateTime.Now, DateTime.Now));
            
            la.AddBar(new Bar(97, 108, 91, 97, 0, DateTime.Now, DateTime.Now));

            Assert.AreEqual(2, la.RefList.Count);
            Assert.AreEqual(89, la.RefList[0].Price);
            Assert.AreEqual(91, la.RefList[1].Price);
        }

        [TestMethod]
        public void References__DownLeg_then_BalanceBar_then_OutsideUpBar_With_Lower_Low()
        {
            LegAnalyzer la = new LegAnalyzer();
            la.AddBar(new Bar(100, 110, 90, 95, 0, DateTime.Now, DateTime.Now));
            la.AddBar(new Bar(99, 109, 89, 94, 0, DateTime.Now, DateTime.Now));
            la.AddBar(new Bar(96, 106, 94, 96, 0, DateTime.Now, DateTime.Now));

            la.AddBar(new Bar(97, 108, 88, 97, 0, DateTime.Now, DateTime.Now));

            Assert.AreEqual(1, la.RefList.Count);
            Assert.AreEqual(88, la.RefList[0].Price);
        }


    }
}
