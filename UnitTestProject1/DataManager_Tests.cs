using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trading.DataManager;
using Trading.Common;
namespace UnitTestProject1
{
    [TestClass]
    public class DataManager_Tests
    {
        [TestMethod]
        public void GetHistoricalDataAsync_writes_Data()
        {
            using (DataManager dm = new DataManager())
            {
                var list = dm.GetHistoricalDataAsync(new Instrument { Name = "USD/CAD", Type = InstrumentType.Forex },
                        new Resolution { TimeFrame = TimeFrame.Hourly, Size = 1 }, new DateTime(2018, 11, 16), new DateTime(2018, 11, 16, 23, 59, 59));
            }
        }

        public void RespondToUserDataRequest_DownloadedData()
        {
            Assert.Fail();
        }
    }
}
