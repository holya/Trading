using System;
using Trading.Common;
using Trading_UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Trading.Common_Tests
{
    [TestClass]
    public class FxBar_Tests
    {

        [TestMethod]
        public void FxBar_Update()
        {
            FxBar upBar = new FxBar(50, 51, 100, 101, 20, 21, 60, 61, 0, DateTime.Now, DateTime.Now);
            upBar.Update(new FxBar(10, 10, 10, 10, 10, 10, 10, 10, 0, DateTime.Now, DateTime.Now));

            Assert.AreEqual(10, upBar.AskHigh);
            Assert.AreEqual(10, upBar.AskLow);
        }
    }
}
