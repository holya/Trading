﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trading.Analyzers.Common;
using Trading.Common;

namespace UnitTestProject1.Trading.Analyzers.Common_Tests
{
    [TestClass]
    public class Utilities_Tests
    {
        [TestMethod]
        public void NormalizeBarDateTime_FXCM_Yearly()
        {
            DateTime actual = Utilities.NormalizeBarDateTime_FXCM(new DateTime(2018, 02, 06, 5, 6, 12), new Resolution(TimeFrame.Yearly, 1));
            DateTime expected = new DateTime(2018, 01, 01, 00, 00, 00, DateTimeKind.Utc);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void NormalizeBarDateTime_FXCM_Quarterly()
        {
            DateTime actual = Utilities.NormalizeBarDateTime_FXCM(new DateTime(2018, 11, 06, 5, 6, 12), new Resolution(TimeFrame.Quarterly, 1));
            DateTime expected = new DateTime(2018, 10, 01, 00, 00, 00, DateTimeKind.Utc);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void NormalizeBarDateTime_FXCM_Monthly()
        {
            DateTime actual = Utilities.NormalizeBarDateTime_FXCM(new DateTime(2018, 02, 06, 5, 6, 12), new Resolution(TimeFrame.Monthly, 1));
            DateTime expected = new DateTime(2018, 02, 01, 00, 00, 00, DateTimeKind.Utc);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void NormalizeBarDateTime_FXCM_Weekly()
        {
            DateTime actual = Utilities.NormalizeBarDateTime_FXCM(new DateTime(2019, 01, 01, 5, 6, 12), new Resolution(TimeFrame.Weekly, 1));
            DateTime expected = new DateTime(2018, 12, 30, 00, 00, 00, DateTimeKind.Utc);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void NormalizeBarDateTime_FXCM_Daily()
        {
            DateTime actual = Utilities.NormalizeBarDateTime_FXCM(new DateTime(2018, 02, 06, 5, 6, 12, DateTimeKind.Local), new Resolution(TimeFrame.Daily, 1));
            DateTime expected = new DateTime(2018, 02, 06, 00, 00, 00, DateTimeKind.Utc);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void NormalizeBarDateTime_FXCM_Hourly()
        {
            DateTime actual = Utilities.NormalizeBarDateTime_FXCM(new DateTime(2018, 01, 01, 5, 59, 59, DateTimeKind.Local), new Resolution(TimeFrame.Hourly, 6));
            DateTime expected = new DateTime(2018, 01, 01, 6, 0, 0);
            Assert.AreEqual(expected, actual);

            actual = Utilities.NormalizeBarDateTime_FXCM(new DateTime(2018, 01, 01, 23, 0, 10), new Resolution(TimeFrame.Hourly, 6));
            expected = new DateTime(2018, 01, 01, 23, 59, 59, 999);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void NormalizeBarDateTime_FXCM_Minute()
        {
            DateTime actual = Utilities.NormalizeBarDateTime_FXCM(new DateTime(2018, 01, 01, 10, 05, 59, DateTimeKind.Local), new Resolution(TimeFrame.Minute, 5));
            DateTime expected = new DateTime(2018, 01, 01, 10, 010, 00, DateTimeKind.Utc);
            Assert.AreEqual(expected, actual);

            actual = Utilities.NormalizeBarDateTime_FXCM(new DateTime(2018, 01, 01, 10, 55, 59), new Resolution(TimeFrame.Minute, 20));
            expected = new DateTime(2018, 01, 01, 10, 59, 59, 999);
            Assert.AreEqual(expected, actual);

            actual = Utilities.NormalizeBarDateTime_FXCM(new DateTime(2018, 01, 01, 23, 58, 25), new Resolution(TimeFrame.Minute, 15));
            expected = new DateTime(2018, 01, 01, 23, 59, 59, 999);
            Assert.AreEqual(expected, actual);
        }
    }
}