using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Trading.Common;

namespace Trading.Common_Tests
{
    [TestClass]
    public class Utilities_Tests
    {
        #region NormalizeAndGetStartDateTime_Minute

        [TestMethod]
        public void NormalizeAndGetStartDateTime_Minute( )
        {

            DateTime actual = Utilities.NormalizeAndGetStartDateTime(new DateTime(2018, 01, 01, 10, 02, 45), new Resolution(TimeFrame.Minute, 5));
            DateTime expected = new DateTime(2018, 01, 01, 10, 0, 0);
            Assert.AreEqual(expected, actual);

            actual = Utilities.NormalizeAndGetStartDateTime(new DateTime(2018, 01, 01, 10, 55, 59, 998), new Resolution(TimeFrame.Minute, 15));
            expected = new DateTime(2018, 01, 01, 10, 45, 0, 0);
            Assert.AreEqual(expected, actual);

            actual = Utilities.NormalizeAndGetStartDateTime(new DateTime(2018, 01, 01, 23, 58, 25), new Resolution(TimeFrame.Minute, 20));
            expected = new DateTime(2018, 01, 01, 23, 40, 0);
            Assert.AreEqual(expected, actual);

            actual = Utilities.NormalizeAndGetStartDateTime(new DateTime(2021, 12, 31, 23, 45, 25), new Resolution(TimeFrame.Minute, 20));
            expected = new DateTime(2021, 12, 31, 23, 40, 0);
            Assert.AreEqual(expected, actual);

            actual = Utilities.NormalizeAndGetStartDateTime(new DateTime(2021, 12, 31, 5, 20, 0), new Resolution(TimeFrame.Minute, 20));
            expected = new DateTime(2021, 12, 31, 5, 20, 0);
            Assert.AreEqual(expected, actual);

            actual = Utilities.NormalizeAndGetStartDateTime(new DateTime(2021, 12, 31, 5, 20, 0), new Resolution(TimeFrame.Minute, 1));
            expected = new DateTime(2021, 12, 31, 5, 20, 0);
            Assert.AreEqual(expected, actual);


        }

        [TestMethod]
        public void NormalizeAndGetStartDateTime_Hourly( )
        {
            DateTime actual;
            DateTime expected;

            actual = Utilities.NormalizeAndGetStartDateTime(new DateTime(2021, 06, 27, 10, 9, 0), new Resolution(TimeFrame.Hourly, 6));
            expected = new DateTime(2021, 06, 27, 6, 0, 0);
            Assert.AreEqual(expected, actual);

            actual = Utilities.NormalizeAndGetStartDateTime(new DateTime(2018, 01, 02, 3, 59, 59), new Resolution(TimeFrame.Hourly, 6));
            expected = new DateTime(2018, 01, 02, 0, 0, 0);
            Assert.AreEqual(expected, actual);

            actual = Utilities.NormalizeAndGetStartDateTime(new DateTime(2018, 04, 01, 23, 9, 0), new Resolution(TimeFrame.Hourly, 6));
            expected = new DateTime(2018, 04, 01, 18, 0, 0);
            Assert.AreEqual(expected, actual);

            actual = Utilities.NormalizeAndGetStartDateTime(new DateTime(2021, 12, 31, 23, 9, 0), new Resolution(TimeFrame.Hourly, 1));
            expected = new DateTime(2021, 12, 31, 23, 0, 0);
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void NormalizeAndGetStartDateTime_Daily( )
        {
            DateTime actual = Utilities.NormalizeAndGetStartDateTime(new DateTime(2018, 02, 03, 5, 6, 12), new Resolution(TimeFrame.Daily, 1));
            DateTime expected = new DateTime(2018, 02, 3, 0, 0, 0);
            Assert.AreEqual(expected, actual);

            actual = Utilities.NormalizeAndGetStartDateTime(new DateTime(2018, 4, 3, 5, 6, 12, DateTimeKind.Local), new Resolution(TimeFrame.Daily, 2));
            expected = new DateTime(2018, 04, 2, 0, 0, 0);
            Assert.AreEqual(expected, actual);

            actual = Utilities.NormalizeAndGetStartDateTime(new DateTime(2021, 6, 1, 21, 6, 12, DateTimeKind.Local), new Resolution(TimeFrame.Daily, 3));
            expected = new DateTime(2021, 5, 30, 0, 0, 0);
            Assert.AreEqual(expected, actual);

            //actual = Utilities.NormalizeBarDateTime_FXCM(new DateTime(2018, 1, 1, 4, 6, 12, DateTimeKind.Local), new Resolution(TimeFrame.Daily, 1));
            //expected = new DateTime(2017, 12, 31, 22, 0, 0);
            //Assert.AreEqual(expected, actual);

        }


        [TestMethod]
        public void NormalizeAndGetStartDateTime_Weekly( )
        {
            DateTime actual;
            DateTime expected;

            actual = Utilities.NormalizeAndGetStartDateTime(new DateTime(2021, 06, 23, 23, 6, 12), new Resolution(TimeFrame.Weekly, 1));
            expected = new DateTime(2021, 6, 20, 0, 00, 00);
            Assert.AreEqual(expected, actual);

            actual = Utilities.NormalizeAndGetStartDateTime(new DateTime(2021, 06, 6, 23, 6, 12), new Resolution(TimeFrame.Weekly, 1));
            expected = new DateTime(2021, 6, 6, 0, 00, 00);
            Assert.AreEqual(expected, actual);

            actual = Utilities.NormalizeAndGetStartDateTime(new DateTime(2021, 12, 25, 23, 6, 12), new Resolution(TimeFrame.Weekly, 1));
            expected = new DateTime(2021, 12, 19, 0, 00, 00);
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void NormalizeAndGetStartDateTime_Monthly( )
        {
            DateTime actual = Utilities.NormalizeAndGetStartDateTime(new DateTime(2018, 02, 06, 5, 6, 12), new Resolution(TimeFrame.Monthly, 1));
            DateTime expected = new DateTime(2018, 2, 1, 0, 0, 0);
            Assert.AreEqual(expected, actual);

            actual = Utilities.NormalizeAndGetStartDateTime(new DateTime(2018, 12, 30, 5, 6, 12), new Resolution(TimeFrame.Monthly, 1));
            expected = new DateTime(2018, 12, 1);
            Assert.AreEqual(expected, actual);
        }


        #endregion

        [TestMethod]
        public void NormalizeAndGetEndDateTime_Minute( )
        {

            DateTime actual = Utilities.NormalizeAndGetEndDateTime(new DateTime(2018, 01, 01, 10, 02, 45), new Resolution(TimeFrame.Minute, 5));
            DateTime expected = new DateTime(2018, 01, 01, 10, 5, 0);
            Assert.AreEqual(expected, actual);

            actual = Utilities.NormalizeAndGetEndDateTime(new DateTime(2018, 01, 01, 10, 55, 59, 998), new Resolution(TimeFrame.Minute, 15));
            expected = new DateTime(2018, 01, 01, 11, 0, 0, 0);
            Assert.AreEqual(expected, actual);

            actual = Utilities.NormalizeAndGetEndDateTime(new DateTime(2018, 01, 01, 23, 58, 25), new Resolution(TimeFrame.Minute, 20));
            expected = new DateTime(2018, 01, 02, 0, 0, 0);
            Assert.AreEqual(expected, actual);

            actual = Utilities.NormalizeAndGetEndDateTime(new DateTime(2021, 12, 31, 23, 45, 25), new Resolution(TimeFrame.Minute, 20));
            expected = new DateTime(2022, 01, 01, 0, 0, 0);
            Assert.AreEqual(expected, actual);

            actual = Utilities.NormalizeAndGetEndDateTime(new DateTime(2021, 12, 31, 5, 20, 0), new Resolution(TimeFrame.Minute, 20));
            expected = new DateTime(2021, 12, 31, 5, 40, 0);
            Assert.AreEqual(expected, actual);

            actual = Utilities.NormalizeAndGetEndDateTime(new DateTime(2021, 12, 31, 5, 20, 0), new Resolution(TimeFrame.Minute, 1));
            expected = new DateTime(2021, 12, 31, 5, 21, 0);
            Assert.AreEqual(expected, actual);


        }

        [TestMethod]
        public void NormalizeAndGetEndDateTime_Hourly( )
        {
            DateTime actual;
            DateTime expected;

            actual = Utilities.NormalizeAndGetEndDateTime(new DateTime(2021, 06, 27, 10, 9, 0), new Resolution(TimeFrame.Hourly, 6));
            expected = new DateTime(2021, 06, 27, 12, 0, 0);
            Assert.AreEqual(expected, actual);

            actual = Utilities.NormalizeAndGetEndDateTime(new DateTime(2018, 01, 02, 3, 59, 59), new Resolution(TimeFrame.Hourly, 6));
            expected = new DateTime(2018, 01, 02, 6, 0, 0);
            Assert.AreEqual(expected, actual);

            actual = Utilities.NormalizeAndGetEndDateTime(new DateTime(2018, 04, 01, 23, 9, 0), new Resolution(TimeFrame.Hourly, 6));
            expected = new DateTime(2018, 04, 02, 0, 0, 0);
            Assert.AreEqual(expected, actual);

            actual = Utilities.NormalizeAndGetEndDateTime(new DateTime(2021, 12, 31, 23, 9, 0), new Resolution(TimeFrame.Hourly, 4));
            expected = new DateTime(2022, 01, 01, 0, 0, 0);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void NormalizeAndGetEndDateTime_Daily( )
        {
            DateTime actual = Utilities.NormalizeAndGetEndDateTime(new DateTime(2018, 02, 03, 5, 6, 12), new Resolution(TimeFrame.Daily, 1));
            DateTime expected = new DateTime(2018, 02, 4, 0, 0, 0);
            Assert.AreEqual(expected, actual);

            //actual = Utilities.NormalizeBarDateTime_FXCM(new DateTime(2018, 4, 3, 5, 6, 12, DateTimeKind.Local), new Resolution(TimeFrame.Daily, 1));
            //expected = new DateTime(2018, 04, 2, 21, 0, 0);
            //Assert.AreEqual(expected, actual);

            //actual = Utilities.NormalizeBarDateTime_FXCM(new DateTime(2018, 4, 3, 21, 6, 12, DateTimeKind.Local), new Resolution(TimeFrame.Daily, 1));
            //expected = new DateTime(2018, 04, 3, 21, 0, 0);
            //Assert.AreEqual(expected, actual);

            //actual = Utilities.NormalizeBarDateTime_FXCM(new DateTime(2018, 1, 1, 4, 6, 12, DateTimeKind.Local), new Resolution(TimeFrame.Daily, 1));
            //expected = new DateTime(2017, 12, 31, 22, 0, 0);
            //Assert.AreEqual(expected, actual);

        }

        [TestMethod]
        public void NormalizeAndGetEndDateTime_Weekly( )
        {
            DateTime actual;
            DateTime expected;

            actual = Utilities.NormalizeAndGetEndDateTime(new DateTime(2021, 06, 23, 23, 6, 12), new Resolution(TimeFrame.Weekly, 1));
            expected = new DateTime(2021, 6, 27, 0, 00, 00);
            Assert.AreEqual(expected, actual);

            actual = Utilities.NormalizeAndGetEndDateTime(new DateTime(2021, 06, 6, 23, 6, 12), new Resolution(TimeFrame.Weekly, 1));
            expected = new DateTime(2021, 6, 13, 0, 00, 00);
            Assert.AreEqual(expected, actual);

            actual = Utilities.NormalizeAndGetEndDateTime(new DateTime(2021, 12, 26, 23, 6, 12), new Resolution(TimeFrame.Weekly, 1));
            expected = new DateTime(2022, 1, 2, 0, 00, 00);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void NormalizeAndGetEndDateTime_Monthly( )
        {
            DateTime actual = Utilities.NormalizeAndGetEndDateTime(new DateTime(2018, 02, 06, 5, 6, 12), new Resolution(TimeFrame.Monthly, 1));
            DateTime expected = new DateTime(2018, 3, 1, 0, 0, 0);
            Assert.AreEqual(expected, actual);

            actual = Utilities.NormalizeAndGetEndDateTime(new DateTime(2018, 12, 30, 5, 6, 12), new Resolution(TimeFrame.Monthly, 1));
            expected = new DateTime(2019, 1, 1);
            Assert.AreEqual(expected, actual);
        }

    }
}
