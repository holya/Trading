using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests;
using Trading.Common;

namespace Trading.Common_Tests
{
    [TestClass]
    public class Pattern_Tests
    {
        [TestMethod]
        public void Construct_First_Pattern()
        {
            var bar = new Bar(10, 20, 5, 15, 0, DateTime.Now, DateTime.Now);

            var p = new Pattern(bar);

            Assert.AreEqual(PatternDirection.Up, p.Direction);
        }
    }
}
