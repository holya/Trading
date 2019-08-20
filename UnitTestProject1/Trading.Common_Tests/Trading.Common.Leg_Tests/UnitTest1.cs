using System;
using Trading_UnitTests;
using Trading.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Trading.Common_Tests
{
    [TestClass]
    public class Leg_Tests
    {
        [TestMethod]
        public void Leg_Direction()
        {
            Leg leg = Helper.GetUpLeg(10);

            Assert.AreEqual(LegDirection.Up, leg.Direction);
        }
    }
}
