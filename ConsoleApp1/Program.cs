﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Brokers.Fxcm;
using Trading.Common;


namespace ConsoleApp1
{
    public class Program
    {
        static void Main(string[] args)
        {

            FxcmWrapper f = new FxcmWrapper();

            f.SessionStatusChanged += (sender, sessionStatusEnum) =>
            {
                Console.WriteLine(f.SessionStatusEnum + "");
            };

            try
            {
                f.Login("U10D2386411", "1786", "http://www.fxcorporate.com/Hosts.jsp", "Demo");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(0);
            }

            DateTime dailyStartDateTime = new DateTime(2018, 4, 1);
            DateTime dailyEndDateTime = new DateTime(2018, 5, 12);
            List<FxBar> dailyBarList = new List<FxBar>();
            try
            {
                dailyBarList = (List<FxBar>)f.GetHistoricalData("USD/CAD", new Resolution(TimeFrame.Daily, 1), dailyStartDateTime, dailyEndDateTime);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(0);
            }


            //var dailyAnalyzer = new LegAnalyzer();
            //dailyAnalyzer.AddBarList(dailyBarList);
            //Console.WriteLine("Daily-----------------------------");
            //Console.WriteLine($"dailyBarList.Count: {dailyBarList.Count}");
            //Console.WriteLine($"dailyBarList.Last().DateTime: {dailyBarList.Last().DateTime}");
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
            //    h6BarList = f.GetHistoricalData("USD/CAD", "H6", h6startDateTime, h6EndDateTime);
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

            Console.WriteLine(f.GetServerTime());

            //Console.ReadLine();
            f.Logout();
        }
    }
}
