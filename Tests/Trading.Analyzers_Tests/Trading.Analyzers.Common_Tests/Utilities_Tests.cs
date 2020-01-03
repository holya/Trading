using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trading.Analyzers.Common;
using Trading.Common;

namespace Trading.Analyzers.Common_Tests
{
    [TestClass]
    public class Utilities_Tests
    {
        [TestMethod]
        public void NormalizeBarDateTime_FXCM_Yearly()
        {
            DateTime actual = Utilities.NormalizeBarDateTime_FXCM(new DateTime(2018, 02, 06, 5, 6, 12), new Resolution(TimeFrame.Yearly, 1));
            DateTime expected = new DateTime(2018, 12, 31, 23, 59, 59, 999);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void NormalizeBarDateTime_FXCM_Quarterly()
        {
            DateTime actual = Utilities.NormalizeBarDateTime_FXCM(new DateTime(2018, 10, 06, 5, 6, 12), new Resolution(TimeFrame.Quarterly, 1));
            DateTime expected = new DateTime(2018, 12, 31, 23, 59, 59, 999);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void NormalizeBarDateTime_FXCM_Monthly()
        {
            DateTime actual = Utilities.NormalizeBarDateTime_FXCM(new DateTime(2018, 02, 06, 5, 6, 12), new Resolution(TimeFrame.Monthly, 1));
            DateTime expected = new DateTime(2018, 1, 31, 22, 0, 0);
            Assert.AreEqual(expected, actual);

            actual = Utilities.NormalizeBarDateTime_FXCM(new DateTime(2018, 1, 30, 5, 6, 12), new Resolution(TimeFrame.Monthly, 1));
            expected = new DateTime(2017, 12, 31, 22, 0, 0);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void NormalizeBarDateTime_FXCM_Weekly()
        {
            DateTime actual;
            DateTime expected;
            actual = Utilities.NormalizeBarDateTime_FXCM(new DateTime(2018, 10, 5, 5, 6, 12), new Resolution(TimeFrame.Weekly, 1));
            expected = new DateTime(2018, 9, 29, 21, 00, 00);
            Assert.AreEqual(expected, actual);

            actual = Utilities.NormalizeBarDateTime_FXCM(new DateTime(2018, 9, 28, 5, 6, 12), new Resolution(TimeFrame.Weekly, 1));
            expected = new DateTime(2018, 9, 22, 21, 00, 00);
            Assert.AreEqual(expected, actual);

            //************** March should return false for DayLightSaving but in this test returns true!!***********//
            actual = Utilities.NormalizeBarDateTime_FXCM(new DateTime(2018, 4, 5, 5, 6, 12), new Resolution(TimeFrame.Weekly, 1));
            expected = new DateTime(2018, 3, 31, 21, 00, 00);
            Assert.AreEqual(expected, actual);


        }

        [TestMethod]
        public void NormalizeBarDateTime_FXCM_Daily()
        {
            DateTime actual = Utilities.NormalizeBarDateTime_FXCM(new DateTime(2018, 02, 03, 5, 6, 12, DateTimeKind.Local), new Resolution(TimeFrame.Daily, 1));
            DateTime expected = new DateTime(2018, 02, 2, 22, 0, 0);
            Assert.AreEqual(expected, actual);

            actual = Utilities.NormalizeBarDateTime_FXCM(new DateTime(2018, 4, 3, 5, 6, 12, DateTimeKind.Local), new Resolution(TimeFrame.Daily, 1));
            expected = new DateTime(2018, 04, 2, 21, 0, 0);
            Assert.AreEqual(expected, actual);

            actual = Utilities.NormalizeBarDateTime_FXCM(new DateTime(2018, 4, 3, 21, 6, 12, DateTimeKind.Local), new Resolution(TimeFrame.Daily, 1));
            expected = new DateTime(2018, 04, 3, 21, 0, 0);
            Assert.AreEqual(expected, actual);

            actual = Utilities.NormalizeBarDateTime_FXCM(new DateTime(2018, 1, 1, 4, 6, 12, DateTimeKind.Local), new Resolution(TimeFrame.Daily, 1));
            expected = new DateTime(2017, 12, 31, 22, 0, 0);
            Assert.AreEqual(expected, actual);

        }

        [TestMethod]
        public void NormalizeBarDateTime_FXCM_Hourly()
        {
            DateTime actual;
            DateTime expected;

            actual = Utilities.NormalizeBarDateTime_FXCM(new DateTime(2018, 01, 01, 23, 9, 0), new Resolution(TimeFrame.Hourly, 6));
            expected = new DateTime(2018, 01, 01, 22, 0, 0);
            Assert.AreEqual(expected, actual);

            actual = Utilities.NormalizeBarDateTime_FXCM(new DateTime(2018, 01, 02, 3, 59, 59), new Resolution(TimeFrame.Hourly, 6));
            expected = new DateTime(2018, 01, 01, 22, 0, 0);
            Assert.AreEqual(expected, actual);

            //////DayTimeSaving times
            actual = Utilities.NormalizeBarDateTime_FXCM(new DateTime(2018, 04, 01, 23, 9, 0), new Resolution(TimeFrame.Hourly, 6));
            expected = new DateTime(2018, 04, 01, 21, 0, 0);
            Assert.AreEqual(expected, actual);

            actual = Utilities.NormalizeBarDateTime_FXCM(new DateTime(2018, 01, 01, 23, 9, 0), new Resolution(TimeFrame.Hourly, 4));
            expected = new DateTime(2018, 01, 01, 22, 0, 0);
            Assert.AreEqual(expected, actual);

            actual = Utilities.NormalizeBarDateTime_FXCM(new DateTime(2018, 01, 01, 2, 0, 10), new Resolution(TimeFrame.Hourly, 1));
            expected = new DateTime(2018, 01, 01, 2, 0, 0);
            Assert.AreEqual(expected, actual);


            //actual = Utilities.NormalizeBarDateTime_FXCM(new DateTime(2018, 01, 02, 2, 0, 10), new Resolution(TimeFrame.Hourly, 6));
            //expected = new DateTime(2018, 01, 01, 22, 0, 0);
            //Assert.AreEqual(expected, actual);

            //actual = Utilities.NormalizeBarDateTime_FXCM(new DateTime(2018, 01, 01, 21, 6, 10), new Resolution(TimeFrame.Hourly, 1));
            //expected = new DateTime(2018, 01, 01, 21, 0, 0);
            //Assert.AreEqual(expected, actual);


        }

        [TestMethod]
        public void NormalizeBarDateTime_FXCM_Minute()
        {
            DateTime actual = Utilities.NormalizeBarDateTime_FXCM(new DateTime(2018, 01, 01, 10, 05, 1), new Resolution(TimeFrame.Minute, 5));
            DateTime expected = new DateTime(2018, 01, 01, 10, 5, 0);
            Assert.AreEqual(expected, actual);

            actual = Utilities.NormalizeBarDateTime_FXCM(new DateTime(2018, 01, 01, 10, 55, 59, 998), new Resolution(TimeFrame.Minute, 15));
            expected = new DateTime(2018, 01, 01, 10, 45, 0, 0);
            Assert.AreEqual(expected, actual);

            actual = Utilities.NormalizeBarDateTime_FXCM(new DateTime(2018, 01, 01, 23, 58, 25), new Resolution(TimeFrame.Minute, 20));
            expected = new DateTime(2018, 01, 01, 23, 40, 0);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DayLightSavingTime()
        {
            bool january = new DateTime(2018, 1, 1).IsDaylightSavingTime();
            bool febuary = new DateTime(2018, 2, 1).IsDaylightSavingTime();
            bool march = new DateTime(2018, 3, 1).IsDaylightSavingTime();
            bool april = new DateTime(2018, 4, 1).IsDaylightSavingTime();
            bool may = new DateTime(2018, 5, 1).IsDaylightSavingTime();
            bool jun = new DateTime(2018, 6, 1).IsDaylightSavingTime();
            bool july = new DateTime(2018, 7, 1).IsDaylightSavingTime();
            bool august = new DateTime(2018, 8, 1).IsDaylightSavingTime();
            bool september = new DateTime(2018, 9, 1).IsDaylightSavingTime();
            bool october = new DateTime(2018, 10, 1).IsDaylightSavingTime();
            bool november = new DateTime(2018, 11, 1).IsDaylightSavingTime();
            bool december = new DateTime(2018, 12, 1).IsDaylightSavingTime();


        }
    }
}
