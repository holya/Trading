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
        public void FirstBar_creates_first_pattern()
        {
            var pa = new PatternAnalyzer();

            pa.AddBar(Helper.GetUpBar());

            Assert.AreEqual(PatternState.Continuation1, pa.LastPattern.State);
            Assert.AreEqual(PatternDirection.Up, pa.LastPattern.Direction);
        }

        [TestMethod]
        public void UpBar_added_to_Continuation1_state_unchanged()
        {
            var pa = new PatternAnalyzer();
            var bar1 = Helper.GetUpBar();
            pa.AddBar(bar1);
            var bar2 = Helper.GetUpBar(bar1, DateTime.Now);
            pa.AddBar(bar2);
            var bar3 = Helper.GetDownBar(bar2, DateTime.Now);
            pa.AddBar(bar3);
            var bar4 = Helper.GetUpBar(bar3, DateTime.Now);
            pa.AddBar(bar4);

            Assert.AreEqual(PatternState.Continuation1, pa.LastPattern.State);
        }
    }
}
