using System;
using Trading.Common;
using Trading_UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Trading.Common_Tests
{
    [TestClass]
    public class Bar_Tests
    {
        [TestMethod]
        public void One_UpBar_Direction_Should_Be_OutsideUp()
        {
            Bar bar1 = new Bar(30, 100, 20, 50, 0, DateTime.Now, DateTime.Now);
            Assert.AreEqual(BarDirection.OutsideUp, bar1.Direction);
        }

        [TestMethod]
        public void One_DownBar_Direction_Should_Be_OutsideDown()
        {
            Bar bar1 = new Bar(50, 100, 20, 30, 0, DateTime.Now, DateTime.Now);
            Assert.AreEqual(BarDirection.OutsideDown, bar1.Direction);
        }


        [TestMethod]
        public void Second_Bar_Direction_Relative_To_PreviousBar__Direction_Up()
        {
            Bar DownBar = new Bar(50, 100, 20, 30, 0, DateTime.Now, DateTime.Now);
            Bar UpBar = new Bar(70, 120, 50, 80, 0, DateTime.Now, DateTime.Now, DownBar);

            Assert.AreEqual(BarDirection.Up, UpBar.Direction);
        }

        [TestMethod]
        public void Second_Bar_Direction_Relative_To_PreviousBar__Direction_GapUp()
        {
            Bar DownBar = new Bar(50, 100, 20, 30, 0, DateTime.Now, DateTime.Now);
            Bar UpBar = new Bar(150, 200, 110, 170, 0, DateTime.Now, DateTime.Now, DownBar);

            Assert.AreEqual(BarDirection.GapUp, UpBar.Direction);
        }


        [TestMethod]
        public void Second_Bar_Direction_Relative_To_PreviousBar__Direction_Down()
        {
            Bar UpBar = new Bar(70, 120, 50, 80, 0, DateTime.Now, DateTime.Now);
            Bar DownBar = new Bar(50, 100, 20, 30, 0, DateTime.Now, DateTime.Now, UpBar);

            Assert.AreEqual(BarDirection.Down, DownBar.Direction);
        }

        [TestMethod]
        public void Second_Bar_Direction_Relative_To_PreviousBar__Direction_GapDown()
        {
            Bar UpBar = new Bar(70, 120, 50, 80, 0, DateTime.Now, DateTime.Now);
            Bar DownBar = new Bar(20, 40, 5, 10, 0, DateTime.Now, DateTime.Now, UpBar);

            Assert.AreEqual(BarDirection.GapDown, DownBar.Direction);
        }

        [TestMethod]
        public void Second_Bar_Direction_Relative_To_PreviousBar__Direction_OutsideUp()
        {
            Bar DownBar = new Bar(50, 100, 20, 30, 0, DateTime.Now, DateTime.Now);
            Bar UpBar = new Bar(60, 120, 10, 70, 0, DateTime.Now, DateTime.Now, DownBar);

            Assert.AreEqual(BarDirection.OutsideUp, UpBar.Direction);
        }

        [TestMethod]
        public void Second_Bar_Direction_Relative_To_PreviousBar__Direction_OutsideDown()
        {
            Bar UpBar = new Bar(70, 120, 50, 80, 0, DateTime.Now, DateTime.Now);
            Bar DownBar = new Bar(60, 130, 10, 50, 0, DateTime.Now, DateTime.Now, UpBar);

            Assert.AreEqual(BarDirection.OutsideDown, DownBar.Direction);
        }


    }
}
