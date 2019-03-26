using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestProject1;

namespace Trading.Pattern_Tests
{
    [TestClass]
    public class Pattern_Tests
    {
        [TestMethod]
        public void PatternType_Validation()
        {
            var bar1 = Helper.GetUpBar();
            Pattern p = new Pattern(bar1);
            var bar2 = Helper.GetUpBar(bar1, bar1.DateTime.AddDays(1));
            p.AddBar(bar2);
            var bar3 = Helper.GetDownBar();
            p.AddBar(bar3);
            var bar4 = Helper.GetDownBar(bar3, bar3.DateTime.AddDays(1));
            p.AddBar(bar4);
            var bar5 = Helper.GetDownBar(bar4, bar4.DateTime.AddDays(1));
            p.AddBar(bar5);
            Assert.AreEqual(State.PullBack1, p.Type);
        }

        [TestMethod]
        public void AddBar_Valid_Directions()
        {
            var b1 = Helper.GetUpBar();
            Pattern p = new Pattern(b1);
            var b2 = (Helper.GetUpBar(b1, b1.DateTime.AddDays(1)));
            p.AddBar(b2);
            var b3 = (Helper.GetUpBar(b2, b2.DateTime.AddDays(1)));
            p.AddBar(b3);
            p.AddBar(Helper.GetDownBar());

            Assert.AreEqual(PatternDirection.Up, p.Direction);
        }

    }
}
