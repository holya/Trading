using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests;
using Trading.Common;

namespace Trading.Common_Tests
{
    [TestClass]
    public class Pattern_Tests
    {
        [TestMethod]
        public void Construct_First_Pattern()
        {
            var bar = new Bar(10, 20, 5, 15, 0, DateTime.Now, DateTime.Now);

            var p = new Pattern(bar);

            Assert.AreEqual(PatternDirection.Up, p.Direction);
        }

        [TestMethod]
        public void With_Three_UpBars_PatternDirection_Should_be_Up()
        {
            Bar bar1 = Helper.GetUpBar();
            Pattern pt = new Pattern(bar1);
            Bar bar2 = Helper.GetUpBar(bar1, DateTime.Now);
            pt.AddBar(bar2);
            Bar bar3 = Helper.GetBalanceBar(bar2, DateTime.Now);
            pt.AddBar(bar3);

            Assert.AreEqual(PatternDirection.Up, pt.Direction);
        }

        [TestMethod]
        public void With_One_UpBar_and_Two_DownBars_PatternDirection_Should_be_Down()
        {
            Bar bar1 = new Bar(10, 20, 0, 5, 0, DateTime.Now, DateTime.Now);
            Pattern pt = new Pattern(bar1);
            Bar bar2 = new Bar(5, 15, -5, 0, 0, DateTime.Now, DateTime.Now);
            pt.AddBar(bar2);
            Bar bar3 = new Bar(0, 10, -10, -5, 0, DateTime.Now, DateTime.Now);
            pt.AddBar(bar3);

            Assert.AreEqual(PatternDirection.Down, pt.Direction);
        }
    }
}
