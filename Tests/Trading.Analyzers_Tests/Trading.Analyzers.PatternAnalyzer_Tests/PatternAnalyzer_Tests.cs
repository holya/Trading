using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;
using Trading.Analyzers.PatternAnalyzer;
using Tests;

namespace Trading.Analyzers_Tests
{
    [TestClass]
    public class PatternAnalyzer_Tests
    {
        [TestMethod]
        public void Two_UpBars_Should_be_Contintuation()
        {
            PatternAnalyzer ptal = new PatternAnalyzer();
            Bar bar1 = new Bar(5, 10, 0, 7, 0, DateTime.Now, DateTime.Now);
            ptal.AddBar(bar1);
            Bar bar2 = new Bar(6, 11, 1, 8, 0, DateTime.Now, DateTime.Now, bar1);
            ptal.AddBar(bar2);

            Assert.AreEqual(PatternState.Continuation, ptal.LastPattern.State);
        }

        [TestMethod]
        public void Two_UpBars_With_One_NonViolating_DownBar_Should_be_PullBack()
        {
            PatternAnalyzer ptal = new PatternAnalyzer();
            Bar bar1 = new Bar(5, 10, 0, 7, 0, DateTime.Now, DateTime.Now);
            ptal.AddBar(bar1);
            Bar bar2 = new Bar(10, 15, 5, 12, 0, DateTime.Now, DateTime.Now, bar1);
            ptal.AddBar(bar2);
            Bar bar3 = new Bar(9, 12, 2, 7, 0, DateTime.Now, DateTime.Now, bar2);
            ptal.AddBar(bar3);

            Assert.AreEqual(PatternState.PullBack, ptal.LastPattern.State);
        }

        [TestMethod]
        public void Two_UpBars_One_Violating_DownBar_Should_be_DownPattern()
        {
            PatternAnalyzer ptal = new PatternAnalyzer();
            Bar bar1 = new Bar(5, 10, 0, 7, 0, DateTime.Now, DateTime.Now);
            ptal.AddBar(bar1);
            Bar bar2 = new Bar(10, 15, 5, 12, 0, DateTime.Now, DateTime.Now, bar1);
            ptal.AddBar(bar2);
            Bar bar3 = new Bar(4, 9, -1, 6, 0, DateTime.Now, DateTime.Now, bar2);
            ptal.AddBar(bar3);

            Assert.AreEqual(PatternDirection.Down, ptal.LastPattern.Direction);
        }
    }
}
