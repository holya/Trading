using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trading.DataManager;

namespace UnitTestProject1.Trading.DataManager_Tests.InstrumentsManager_Tests
{
    [TestClass]
    public class InstrumentsManager_Tests
    {
        [TestMethod]
        public void GetForexPairs__get_major_pairs()
        {
            InstrumentsManager im = new InstrumentsManager();

            IEnumerable<string> majorList = im.GetForexPairs(ForexTypes.major);

            Assert.AreEqual(7, majorList.Count());
        }
    }
}
