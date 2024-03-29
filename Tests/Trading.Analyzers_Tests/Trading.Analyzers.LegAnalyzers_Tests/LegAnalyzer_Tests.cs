﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trading.Analyzers.LegAnalyzer;
using Trading.Common;
using Tests;
namespace Trading.Analyzers_Tests
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
        public void OutsideUpBar_After_UpLeg_Should_Create_New_UpLeg()
        {
            LegAnalyzer la = new LegAnalyzer();

            la.AddBar(new Bar(10, 20, 5, 12, 0, DateTime.Now));
            la.AddBar(new Bar(15, 25, 10, 17, 0, DateTime.Now));

            la.AddBar(new Bar(15, 27, 8, 17, 0, DateTime.Now));

            Assert.AreEqual(2, la.LegsCount);
            Assert.AreEqual(LegDirection.Up, la.LastLeg.Direction);

            //Assert.AreEqual(2, la.RefList.Count);
            //Assert.AreEqual(25, la.RefList[0].Price);
            //Assert.AreEqual(8, la.RefList[1].Price);
        }

        //[TestMethod]
        //public void References__DownLeg_then_UpBar()
        //{
        //    LegAnalyzer la = new LegAnalyzer();
        //    la.AddBar(new Bar(100, 110, 90, 95, 0, DateTime.Now, DateTime.Now));
        //    la.AddBar(new Bar(99, 109, 89, 94, 0, DateTime.Now, DateTime.Now));

        //    la.AddBar(new Bar(100, 112, 91, 94, 0, DateTime.Now, DateTime.Now));

        //    Assert.AreEqual(1, la.RefList.Count);
        //    Assert.AreEqual(89, la.RefList[0].Price);
        //}

        //[TestMethod]
        //public void References__DownLeg_then_OutsideUpBar()
        //{
        //    LegAnalyzer la = new LegAnalyzer();
        //    la.AddBar(new Bar(100, 110, 90, 95, 0, DateTime.Now, DateTime.Now));
        //    la.AddBar(new Bar(99, 109, 89, 94, 0, DateTime.Now, DateTime.Now));

        //    la.AddBar(new Bar(94, 112, 88, 95, 0, DateTime.Now, DateTime.Now));

        //    Assert.AreEqual(1, la.RefList.Count);
        //    Assert.AreEqual(88, la.RefList[0].Price);
        //}

        //[TestMethod]
        //public void References__DownLeg_then_BalanceBar_then_UpBar()
        //{
        //    LegAnalyzer la = new LegAnalyzer();
        //    la.AddBar(new Bar(100, 110, 90, 95, 0, DateTime.Now, DateTime.Now));
        //    la.AddBar(new Bar(99, 109, 89, 94, 0, DateTime.Now, DateTime.Now));
        //    la.AddBar(new Bar(98, 108, 90, 94, 0, DateTime.Now, DateTime.Now));

        //    la.AddBar(new Bar(97, 112, 91, 95, 0, DateTime.Now, DateTime.Now));

        //    Assert.AreEqual(1, la.RefList.Count);
        //    Assert.AreEqual(89, la.RefList[0].Price);
        //}

        //[TestMethod]
        //public void References__DownLeg_then_BalanceBar_then_OutsideUpBar_With_Higher_Low()
        //{
        //    LegAnalyzer la = new LegAnalyzer();
        //    la.AddBar(new Bar(100, 110, 90, 95, 0, DateTime.Now, DateTime.Now));
        //    la.AddBar(new Bar(99, 109, 89, 94, 0, DateTime.Now, DateTime.Now));
        //    la.AddBar(new Bar(96, 106, 94, 96, 0, DateTime.Now, DateTime.Now));
            
        //    la.AddBar(new Bar(97, 108, 91, 97, 0, DateTime.Now, DateTime.Now));

        //    Assert.AreEqual(2, la.RefList.Count);
        //    Assert.AreEqual(89, la.RefList[0].Price);
        //    Assert.AreEqual(91, la.RefList[1].Price);
        //}

        //[TestMethod]
        //public void References__DownLeg_then_BalanceBar_then_OutsideUpBar_With_Lower_Low()
        //{
        //    LegAnalyzer la = new LegAnalyzer();
        //    la.AddBar(new Bar(100, 110, 90, 95, 0, DateTime.Now, DateTime.Now));
        //    la.AddBar(new Bar(99, 109, 89, 94, 0, DateTime.Now, DateTime.Now));
        //    la.AddBar(new Bar(96, 106, 94, 96, 0, DateTime.Now, DateTime.Now));

        //    la.AddBar(new Bar(97, 108, 88, 97, 0, DateTime.Now, DateTime.Now));

        //    Assert.AreEqual(1, la.RefList.Count);
        //    Assert.AreEqual(88, la.RefList[0].Price);
        //}


    }
}
