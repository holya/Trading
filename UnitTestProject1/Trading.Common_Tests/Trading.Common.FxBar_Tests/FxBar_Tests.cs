﻿using System;
using Trading.Common;
using Trading_UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Trading.Common_Tests
{
    [TestClass]
    public class FxBar_Tests
    {
        [TestMethod]
        public void FxBar_with_PreviousBar_Functionality_as_expected_case_UpBar_to_DownBar()
        {
            FxBar upBar = new FxBar(50, 51, 100, 101, 20, 21, 60, 61, 0, DateTime.Now, DateTime.Now);
            FxBar downBar = new FxBar(30, 31, 70, 71, 10, 11, 20, 21, 0, DateTime.Now, DateTime.Now, upBar);

            Assert.AreEqual(BarDirection.Down, downBar.Direction);
        }

        [TestMethod]
        public void FxBar_Update()
        {
            FxBar upBar = new FxBar(50, 51, 100, 101, 20, 21, 60, 61, 0, DateTime.Now, DateTime.Now);

            upBar.Update(new FxBar(10, 10, 10, 10, 10, 10, 10, 10, 0, DateTime.Now, DateTime.Now));
        }
    }
}
