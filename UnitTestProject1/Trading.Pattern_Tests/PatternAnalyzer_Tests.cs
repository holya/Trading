using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;
using Trading.Analyzers.PatternAnalyzer;
using UnitTestProject1;

namespace Trading.Pattern_Tests
{
    [TestClass]
    public class PatternAnalyzer_Tests
    {
        [TestMethod]
        public void Reference_Validation()
        {
            var bar1 = Helper.GetUpBar();
            PatternAnalyzer pa = new PatternAnalyzer();
            var bar2 = Helper.GetUpBar(bar1, bar1.DateTime.AddDays(1));
            pa.AddBar(bar2);
            var bar3 = Helper.GetDownBar();
            pa.AddBar(bar3);
            var bar4 = Helper.GetDownBar(bar3, bar3.DateTime.AddDays(1));
            pa.AddBar(bar4);
            var bar5 = Helper.GetDownBar(bar4, bar4.DateTime.AddDays(1));
            pa.AddBar(bar5);

            var refCount = pa.RefList.Count;
            Assert.AreEqual(2, refCount);
        }
    }
}
