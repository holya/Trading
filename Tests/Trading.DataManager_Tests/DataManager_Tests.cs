using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trading.DataManager;
using Trading.Common;
using Moq;
using Trading.DataProviders.Common;
using Trading.DataBases.Common;


namespace Trading.DataManager_Tests
{ 
    [TestClass]
    public class DataManager_Tests
    {
        [TestMethod]
        public void GetHystoricalDataAsync__beginDate_Greater_than_Last_Local_Date()
        {
            var dpMoq = new Mock<IDataProvider>();

            Instrument instrument = new Instrument { Name = "USDCAD", Type = InstrumentType.Forex };
            Resolution resolution = new Resolution(TimeFrame.Minute, 1);

            dpMoq.Setup(x => x.SubscribeRealTime(instrument));
            //dpmock.SetupAdd(m => m.FooEvent += It.IsAny<EventHandler>())...;
            
            var dbMoq = new Mock<IDataBase>();

            var dm = new DataManager.DataManager(dpMoq.Object, dbMoq.Object);
            dm.RealTimeTickUpdated += (o, args) =>
            {

            };
            
            dm.SubscribeRealTime(instrument, resolution);
            dm.SubscribeRealTime(new Instrument("USDJPY", InstrumentType.Forex), new Resolution(TimeFrame.Minute, 2));

            dpMoq.SetupAdd(m => m.RealTimeDataUpdated += (object o, RealTimeDataUpdatedEventArgs e) => { });
            

            dpMoq.Raise(m => m.RealTimeDataUpdated += null, new RealTimeDataUpdatedEventArgs(
                new Instrument("USDCAD", InstrumentType.Forex), 10, 20, DateTime.UtcNow));
            
        }

    }
}
