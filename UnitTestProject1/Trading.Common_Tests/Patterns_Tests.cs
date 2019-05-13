using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trading.Common;

namespace Trading.Common_Tests
{
    [TestClass]
    public class Pattern_Tests
    {
        [TestMethod]
        public void AddBar_Two_UpBars()
        {
            var bar1 = new Bar(30, 100, 20, 50, 0, DateTime.Now, DateTime.Now);
            var p = new Pattern(bar1);
            var bar2 = new Bar(40, 110, 30, 60, 0, DateTime.Now, DateTime.Now);
            var result = p.AddBar(bar2);

            Assert.IsTrue(result);
            Assert.AreEqual(2, p.LastLeg.BarCount);
        }
    }
}
