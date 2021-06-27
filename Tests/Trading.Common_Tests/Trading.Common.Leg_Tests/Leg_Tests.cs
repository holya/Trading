using System;
using Trading.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests;

namespace Trading.Common_Tests
{
    [TestClass]
    public class Leg_Tests
    {
        [TestMethod]
        public void UpBars_Should_Make_UpLeg()
        {
            Leg upLeg = Helper.GetUpLeg(10);

            Assert.AreEqual(LegDirection.Up, upLeg.Direction);


        }

        [TestMethod]
        public void DownBars_Should_Make_DownLeg()
        {
            Leg downLeg = Helper.GetDownLeg(10);

            Assert.AreEqual(LegDirection.Down, downLeg.Direction);
        }

        [TestMethod]
        public void Adding_DownBar_to_UpLeg_Should_return_False()
        {
            Leg upLeg = Helper.GetUpLeg(5);
            var result = upLeg.AddBar(new FxBar(7, 8, 10, 11, 0, 1, 5, 6, 0, DateTime.Now, DateTime.Now));

            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void Adding_UpBar_to_DownLeg_Should_return_False()
        {
            Leg downLeg = Helper.GetDownLeg(5);
            var result = downLeg.AddBar(new FxBar(80, 81, 120, 121, 50, 51, 90, 91, 0, DateTime.Now, DateTime.Now));

            Assert.AreEqual(false, result);
        }


        [TestMethod]
        public void Adding_UpBar_to_UpLeg_Should_Add_Successfully()
        {
            Leg upLeg = Helper.GetUpLeg(3);
            upLeg.AddBar(new FxBar(80, 81, 120, 121, 50, 51, 90, 91, 0, DateTime.Now, DateTime.Now));

            Assert.AreEqual(4, upLeg.BarCount);
        }

        [TestMethod]
        public void Adding_DownBar_to_DownLeg_Should_Add_Successfully()
        {
            Leg downLeg = Helper.GetDownLeg(3);
            downLeg.AddBar(new FxBar(7, 8, 10, 11, 0, 1, 5, 6, 0, DateTime.Now, DateTime.Now));

            Assert.AreEqual(4, downLeg.BarCount);
        }

        [TestMethod]
        public void UpLeg_With_Previous_DownLeg_BarCount()
        {
            Leg upLeg = Helper.GetUpLeg(10);
            Leg downLeg = Helper.GetDownLeg(12, upLeg);

            Assert.AreEqual(10, downLeg.PreviousLeg.BarCount);
        }

    }
}
