using System;
using System.Collections.Generic;
using Trading.Common;
using Trading.Brokers.Fxcm;
using Trading.Analyzers.LegAnalyzer;
using System.Linq;
using System.Threading.Tasks;
using Trading.Analyzers.Common;
using Trading;
using Unity;
using Trading.DataBases.XmlDataBase;
using Trading.DataProviders.Common;
using System.Diagnostics;
using Trading.DataManager;
using Trading.DataManager.Common;
namespace ConsoleApp1
{
    public class Program
    {
        public  static async Task Main(string[] args)
        {
            #region IOC Unity
            //Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =
            //    {
            //        var c = new UnityContainer();
            //        ContainerBootStrapper.RegisterTypes(c);
            //        return c;
            //    });
            //var container = new UnityContainer();
            //ContainerBootStrapper.RegisterTypes(container);

            //var dm = container.Value.Resolve<DataManager>();
            #endregion

            await TestDatamanager();
        }

        static async Task TestDatamanager()
        {
            var dm2 = new DataManager(new FxcmWrapper(), new XmlDataBase());
            dm2.RealTimeDataUpdated += (object sender, RTDataUpdateEventArgs e) => Console.WriteLine($"Bid: {e.Price}---DateTime: {e.DateTime}");

            Console.WriteLine("Logging in...");
            var res = await dm2.Login("U10D2442130", "7400", "http://www.fxcorporate.com/Hosts.jsp", "Demo");
            Console.WriteLine($"Logged in:{res.SessionStatus}");

            var instrument = new Instrument { Name = "GBPJPY", Type = InstrumentType.Forex };
            var resolution = new Resolution(TimeFrame.Hourly, 1);


            do
            {
                #region real-time sunscription
                //Console.WriteLine("Sunscribing to realtime\n");
                //dm2.SubscribeRealTime(instrument);
                //await Task.Delay(8000);
                //dm2.UnsubscribeRealTime(instrument);
                //Console.WriteLine("\nUnsubscribing f-rom realtime");
                #endregion
                var startDT = new DateTime(2021, 06, 16, 6, 0, 0, DateTimeKind.Utc); //DateTime.UtcNow.AddDays(-1);
                var endDT = new DateTime(2021, 06, 16, 12, 0, 0, DateTimeKind.Utc); //DateTime.UtcNow;

                List<Bar> barList = new List<Bar>();
                try
                {
                    barList.AddRange(await dm2.GetHistoricalDataAsync(instrument, resolution, startDT, endDT));
                }
                catch (TimeframeNotFoundException ex)
                {
                    Console.WriteLine("\n" + ex.Message);
                    Console.WriteLine("\n\nHit Enter to continue or 'c' to finish:");
                    continue;
                }
                Console.WriteLine("6. "+(await dm2.GetHistoricalDataAsync(instrument, resolution, startDT, endDT)).Count());
                //////////////////////////////////////////////
                
                //var bars = await dm2.GetHistoricalDataAsync(instrument, resolution, startDT.AddHours(-3), startDT);
                //if(bars.Last().DateTime.AddHours(1) != startDT)
                //{
                //    Console.WriteLine("startDate and endDate before last local dateTime");
                //    Console.ReadKey();
                //}
                //Console.WriteLine("9. " + (await dm2.GetHistoricalDataAsync(instrument, resolution, startDT.AddHours(-3), endDT)).Count());
                //////////////////////////////////////////////////
                

                //bars = await dm2.GetHistoricalDataAsync(instrument, resolution, startDT.AddHours(-4), startDT.AddHours(1));
                //if(bars.Count() != 5)
                //{
                //    Console.WriteLine($"There should be 5 bars, lastDateTime:{bars.Last().DateTime}");
                //    Console.ReadKey();
                //}
                //Console.WriteLine("10. " + (await dm2.GetHistoricalDataAsync(instrument, resolution, startDT.AddHours(-4), endDT)).Count());
                /////////////////////////////////////////////////////
                

                //bars = await dm2.GetHistoricalDataAsync(instrument, resolution, startDT.AddHours(-3), endDT);
                //if(bars.Count() != 11)
                //{
                //    Console.WriteLine("There should be 11 bars returned");
                //    Console.ReadKey();
                //}
                //if(bars.Last().DateTime != endDT.AddHours(-1))
                //{
                //    Console.WriteLine($"The last bar's datetime should be: {endDT}");
                //}
                //Console.WriteLine("12. " + (await dm2.GetHistoricalDataAsync(instrument, resolution, startDT.AddHours(-4), endDT)).Count());
                ////////////////////////////////////////////////////////
                

                //bars = await dm2.GetHistoricalDataAsync(instrument, resolution, startDT, endDT.AddHours(4));
                //if(bars.Count() != 12)
                //{
                //    Console.WriteLine("There should be 12 bars!");
                //}
                //if(bars.Last().DateTime.Hour != 17)
                //{
                //    Console.WriteLine("DateTime is incorrect!");
                //}
                //Console.WriteLine("16. " + (await dm2.GetHistoricalDataAsync(instrument, resolution, startDT.AddHours(-4), endDT.AddHours(4))).Count());
                //////////////////////////////////////////////////////////



                var bars = await dm2.GetHistoricalDataAsync(instrument, resolution, endDT.AddHours(1), endDT.AddHours(5));



                Console.WriteLine("\n\nHit Enter to continue or 'c' to finish:");

            } while (Console.ReadKey().KeyChar != 'c');

            
            Console.WriteLine("\n\nGood bye!!");
            dm2.Logout();

        }


        private static void Dm2_RealTimeDataUpdated(object sender, RealTimeDataUpdatedEventArgs e)
        {
            Console.WriteLine(e);
        }

        private static void Dm_SessionStatusChanged(object sender, SessionStatusChangedEventArgs e)
        {
            Console.WriteLine($"{e.SessionStatus.ToString()}");
        }
    }
}
