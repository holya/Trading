using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;
using UnitTestProject1;

namespace Trading.PatternAnalyzer_Tests
{
    [TestClass]
    class PatternAnalyzer_Tests
    {
        [TestMethod]
        void AddBar_Valid_Directions()
        {
            Pattern p = new Pattern();
            List<Bar> barlist = new List<Bar>();
            barlist.Add(Helper.GetUpBar());
            barlist.Add(Helper.GetUpBar(barlist.Last(), barlist.Last().DateTime.AddDays(1)));
            barlist.Add(Helper.GetUpBar(barlist.Last(), barlist.Last().DateTime.AddDays(1)));
            barlist.Add(Helper.GetDownBar());

            foreach (var b in barlist)
            {
                p.LegList.Add(new Leg(b));
            }

            Assert.AreEqual(p.Direction, LegDirection.Down);
        }
    }
}
