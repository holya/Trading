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
            var bar1 = new Bar(30, 100, 20, 50, 0, DateTime.Now, DateTime.Now);
            pa.AddBar(bar1);
            var bar2 = new Bar(50, 140, 40, 60, 0, DateTime.Now, DateTime.Now);
            pa.AddBar(bar2);
            var bar3 = new Bar(10, 90, 5, 30, 0, DateTime.Now, DateTime.Now);
            pa.AddBar(bar3);
            var bar4 = new Bar(20, 120, 30, 40, 0, DateTime.Now, DateTime.Now);
            pa.AddBar(bar4);

            Assert.AreEqual(PatternState.Continuation1, pa.LastPattern.State);
        }

        [TestMethod]
        public void Continuation1_changed_to_Continuation2()
        {
            var pa = new PatternAnalyzer();
            var bar1 = new Bar(30, 100, 20, 50, 0, DateTime.Now, DateTime.Now);
            pa.AddBar(bar1);
            var bar2 = new Bar(50, 140, 40, 60, 0, DateTime.Now, DateTime.Now);
            pa.AddBar(bar2);
            var bar3 = new Bar(40, 90, 25, 30, 0, DateTime.Now, DateTime.Now);
            pa.AddBar(bar3);
            var bar4 = new Bar(40, 150, 30, 60, 0, DateTime.Now, DateTime.Now);
            pa.AddBar(bar4);

            Assert.AreEqual(PatternState.Continuation2, pa.LastPattern.State);
        }

        [TestMethod]
        public void Continuation1_changed_to_PullBack1()
        {
            var pa = new PatternAnalyzer();
            var bar1 = new Bar(30, 100, 20, 50, 0, DateTime.Now, DateTime.Now);
            pa.AddBar(bar1);
            var bar2 = new Bar(35, 105, 25, 55, 0, DateTime.Now, DateTime.Now);
            pa.AddBar(bar2);
            var bar3 = new Bar(40, 90, 22, 35, 0, DateTime.Now, DateTime.Now);
            pa.AddBar(bar3);

            Assert.AreEqual(PatternState.PullBack1, pa.LastPattern.State);
        }

        [TestMethod]
        public void Continuation2_to_Pullback1()
        {
            var pa = new PatternAnalyzer();
            var bar1 = new Bar(30, 100, 20, 50, 0, DateTime.Now, DateTime.Now);
            pa.AddBar(bar1);
            var bar2 = new Bar(40, 110, 30, 60, 0, DateTime.Now, DateTime.Now);
            pa.AddBar(bar2);
            var bar3 = new Bar(55, 105, 25, 35, 0, DateTime.Now, DateTime.Now);
            pa.AddBar(bar3);
            var bar4 = new Bar(50, 120, 40, 70, 0, DateTime.Now, DateTime.Now);
            pa.AddBar(bar4);
            var bar5 = new Bar(65, 115, 41, 45, 0, DateTime.Now, DateTime.Now);
            pa.AddBar(bar5);

            Assert.AreEqual(PatternState.PullBack1, pa.LastPattern.State);
        }

        [TestMethod]
        public void Up_Pattern_to_Down_Pattern_Support_Violated()
        {
            var pa = new PatternAnalyzer();
            var bar1 = new Bar(30, 100, 20, 50, 0, DateTime.Now, DateTime.Now);
            pa.AddBar(bar1);
            var bar2 = new Bar(40, 110, 30, 60, 0, DateTime.Now, DateTime.Now);
            pa.AddBar(bar2);
            var bar3 = new Bar(40, 50, 10, 20, 0, DateTime.Now, DateTime.Now);
            pa.AddBar(bar3);

            Assert.AreEqual(PatternDirection.Down, pa.LastPattern.Direction);
        }
    }
}
