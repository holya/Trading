using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;
using Trading.Analyzers.PatternAnalyzer;
//using Trading_UnitTests;

namespace Trading.Analyzers_Tests
{
    [TestClass]
    public class PatternAnalyzer_Tests
    {
        [TestMethod]
        public void Add_New_UpBar_to_DownPattern_With_Reference_Violation()
        {
            Bar bar = new Bar(5, 15, -5, 0, 0, DateTime.Now, DateTime.Now);
            Pattern pt = new Pattern(bar);
            Bar bar2 = new Bar(0, 10, -10, -5, 0, DateTime.Now, DateTime.Now);
            pt.AddBar(bar2);

        }
    }
}
