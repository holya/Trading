using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;
using Trading.Analyzers.PatternAnalyzer;
using UnitTestProject1;

namespace Trading.PatternAnalyzer_Tests
{
    [TestClass]
    public class Pattern_Tests
    {
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
