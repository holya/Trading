using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestProject1;

namespace Trading.Patterns_Tests
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
            Assert.AreEqual(PatternType.PullBack1, p.Type);
        }
    }
}
