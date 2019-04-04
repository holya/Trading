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
        public void FirstBar_creates_firs_pattern()
        {
            var pa = new PatternAnalyzer();

            pa.AddBar(Helper.GetUpBar());

            Assert.AreEqual(PatternState.Continuation1 ,pa.LastPattern.State);
        }
    }
}
