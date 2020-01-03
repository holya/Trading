using System;
using System.Collections.Generic;
using Trading.Common;
using Trading.Brokers.Fxcm;
using Trading.Analyzers.LegAnalyzer;
using System.Linq;
using System.Threading.Tasks;
using Trading.Analyzers.Common;
using Trading.DataManager;
using Trading;
using Unity;
using Trading.DataBases.MongoDb;
using Trading.DataProviders.ActiveTick;
using Trading.DataBases.XmlDataBase;
using Trading.DataProviders.Common;

namespace ConsoleApp1
{
    public class Program
    {
        public  static async Task Main(string[] args)
        {
            //Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
            //    {
            //        var c = new UnityContainer();
            //        ContainerBootStrapper.RegisterTypes(c);
            //        return c;
            //    });
            //var container = new UnityContainer();
            //ContainerBootStrapper.RegisterTypes(container);

            //var dm = container.Value.Resolve<DataManager>();

            var dm = new ActiveTick();
            //var dm2 = new FxcmWrapper();

            //var dm = new DataManager(new ActiveTick(), new XmlDataBase());

            //dm.SessionStatusChanged += Dm_SessionStatusChanged;

            var r = await dm.Login("holya", "maryam");

            //var res = await dm2.Login("U10D2386411", "1786", "http://www.fxcorporate.com/Hosts.jsp", "Demo");
            //Console.WriteLine(res);
            //Instrument instrument = new Instrument { Name = "AAPL", Type = InstrumentType.Stock };
            Instrument instrument = new Instrument { Name = "USDCAD", Type = InstrumentType.Forex };
            var ed = DateTime.Now.AddDays(1);
            var sd = ed.AddDays(-10);

            var barList = new List<Bar>();
            var barList2 = new List<Bar>();
            try
            {
                var bars = await dm.GetHistoricalDataAsync(instrument, new Resolution(TimeFrame.Daily, 1), sd, ed);
                barList.AddRange(bars);

                //var bars2 = await dm2.GetHistoricalDataAsync(instrument, new Resolution(TimeFrame.Daily, 1), sd, ed);
                //barList2.AddRange(bars2);

            }
            catch (HistoricalDataDownloadException e)
            {
                Console.WriteLine(e.Message);
            }
            //dm.RealTimeDataUpdated += (object sender, RealTimeDataUpdatedEventArgs e) =>
            //{
            //    var tuple = (Tuple<string, double, double, DateTime, int>)e.Data;

            //    Console.WriteLine($"symbol: {tuple.Item1}\nBid: {tuple.Item2}\nAsk: {tuple.Item3}\nDateTime: {tuple.Item4}\nSize: {tuple.Item5}");
            //};

            Console.WriteLine(barList.Count);
            Console.WriteLine(barList2.Count);
            Console.WriteLine(barList.First().DateTime);
            Console.WriteLine(barList.Last().EndDateTime);

            Console.WriteLine(barList2.Last().DateTime);
            Console.WriteLine(barList2.Last().EndDateTime);

            //dm.SubscribeToRealTime("USDCAD");
            //dm.SubscribeToRealTime("#USD/CAD");

            Console.WriteLine($"Count: {barList.Count}\n");
            foreach (var bar in barList)
                Console.WriteLine(bar.ToString());
            Console.WriteLine("__________________________");
            foreach (var bar in barList2)
                Console.WriteLine(bar.ToString());

            //dm.Logout();
            //dm2.Logout();
            Console.ReadLine();
            //dm.Dispose();



            Console.WriteLine("done");
            
            //try
            //{
            //    f.Login("U10D2386411", "1786", "http://www.fxcorporate.com/Hosts.jsp", "Demo");
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //    Environment.Exit(0);
            //}

            //DateTime dailyEndDateTime = new DateTime(now.Year, now.Month, , 0, 0, 0);
            //try
            //{
            //    dailyBarList = (List<FxBar>)f.GetHistoricalDataAsync(instrument, new Resolution(TimeFrame.Daily, 1), dailyStartDateTime, dailyEndDateTime).GetAwaiter().GetResult();
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //    //Environment.Exit(0);

            //}



            //var dailyAnalyzer = new LegAnalyzer();
            //dailyAnalyzer.AddBarList(dailyBarList);
            //int k = dailyAnalyzer.BarsCount;

            //int k2 = 1;


            //Console.WriteLine("Daily-----------------------------");
            //Console.WriteLine($"dailyBarList.Count: {dailyBarList.Count}");
            //Console.WriteLine($"dailyBarList.Last().DateTime: {dailyBarList.Last().DateTime}");
            //Console.WriteLine($"First bar time: { dailyBarList.First().DateTime}");
            //Console.WriteLine($"Daily StartDateTime: {dailyStartDateTime}");
            //Console.WriteLine($"Daily EndDateTime: {dailyEndDateTime}");
            //Console.WriteLine($"Close: {dailyAnalyzer.Close}");
            //Console.WriteLine("-----------------------------");



            //var lastDailyAnalyzerLeg = dailyAnalyzer.LastLeg;
            //DateTime h6startDateTime = lastDailyAnalyzerLeg.StartDateTime;
            //DateTime h6EndDateTime = lastDailyAnalyzerLeg.LastBar.DateTime.AddDays(1);
            //List<FxBar> h6BarList = new List<FxBar>();
            //try
            //{
            //    h6BarList = (List<FxBar>)f.GetHistoricalData(symbol, new Resolution(TimeFrame.Hourly, 6), h6startDateTime, h6EndDateTime);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //    Environment.Exit(0);
            //}
            //var h6Analyzer = new LegAnalyzer();
            //h6Analyzer.AddBarList(h6BarList);
            //Console.WriteLine("h6-----------------------------");
            //Console.WriteLine($"h6.BarList.Count: {h6BarList.Count}\nh6BarList.Last().DateTime: {h6BarList.Last().DateTime}");
            //Console.WriteLine($"H6 StartDateTime: {h6startDateTime}");
            //Console.WriteLine($"H6 EndDateTime: {h6EndDateTime}");
            //Console.WriteLine($"Close: {h6Analyzer.Close}");
            //Console.WriteLine("-----------------------------");


            //var lasth6AnalyzerLeg = h6Analyzer.LastLeg;
            //DateTime h1startDateTime = lasth6AnalyzerLeg.StartDateTime;
            //DateTime h1EndDateTime = DateTime.Now.AddHours(7);//lasth6AnalyzerLeg.LastBar.DateTime.AddHours(6);
            //List<FxBar> h1BarList = new List<FxBar>();
            //try
            //{
            //    h1BarList = f.GetHistoricalData("USD/CAD", "H1", h1startDateTime, h1EndDateTime);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //    Environment.Exit(0);
            //}
            //var h1Analyzer = new LegAnalyzer();
            //h1Analyzer.AddBarList(h1BarList);
            //Console.WriteLine("h1-----------------------------");
            //Console.WriteLine($"h1BarList.Count: {h1BarList.Count}\nh1.BarList.Last().DateTime: {h1BarList.Last().DateTime}");
            //Console.WriteLine($"H1 StartDateTime: {h1startDateTime}");
            //Console.WriteLine($"H1 EndDateTime: {h1EndDateTime}");
            //Console.WriteLine($"Close: {h1Analyzer.Close}");
            //Console.WriteLine("-----------------------------");



            //foreach (var bar in dayBl)
            //{
            //    Console.WriteLine($"BidOpen: {bar.Open} BidHigh: {bar.High} BidLow: {bar.Low} BidClose: {bar.Close} " +
            //                      $"AskOpen: {bar.AskOpen} AskHigh: {bar.AskHigh} AskLow: {bar.AskLow} AskClose: {bar.AskClose} " +
            //                      $"Volume: {bar.Volume} DateTime: {bar.DateTime}\n.........................");
            //}


            //Console.WriteLine($"Bar Count: {h6anal.LastLeg.BarCount}");

            //Console.WriteLine(f.GetServerTime());

            //f.Logout();

        }


        private static void Dm_SessionStatusChanged(object sender, SessionStatusChangedEventArgs e)
        {
            Console.WriteLine($"{e.SessionStatus.ToString()}");
        }
    }
}
