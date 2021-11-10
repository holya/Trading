using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;
namespace Trading.Common
{
    public static class Utilities
    {
        public static DateTime GetEndDateTime(DateTime dateTime, Resolution resolution)
        {
            switch (resolution.TimeFrame)
            {
                case TimeFrame.Minute:
                    return dateTime.AddMinutes(resolution.Size);
                case TimeFrame.Hourly:
                    return dateTime.AddHours(resolution.Size);
                case TimeFrame.Daily:
                    return dateTime.AddDays(resolution.Size);
                case TimeFrame.Weekly:
                    return dateTime.AddDays(7);
                case TimeFrame.Monthly:
                    return dateTime.AddMonths(resolution.Size);
                case TimeFrame.Quarterly:
                    return dateTime.AddMonths(resolution.Size * 3);
                case TimeFrame.Yearly:
                    return dateTime.AddYears(resolution.Size);
                default:
                    return dateTime;
            }
        }

        public static DateTime NormalizeAndGetEndDateTime(DateTime date, Resolution resolution)
        {
            switch (resolution.TimeFrame)
            {
                case TimeFrame.Yearly:
                    return new DateTime(date.Year, 01, 01).AddYears(1).AddMilliseconds(-1);

                case TimeFrame.Quarterly:
                    int month;
                    if (date.Month < 4)
                        month = 1;
                    else if (date.Month < 7)
                        month = 4;
                    else if (date.Month < 10)
                        month = 7;
                    else
                        month = 10;
                    return new DateTime(date.Year, month, 1).AddMonths(3).AddMilliseconds(-1);

                case TimeFrame.Monthly:
                    return GetEndDateTime(new DateTime(date.Year, date.Month, 1, 0, 0, 0), resolution);

                case TimeFrame.Weekly:
                    var tempDt = date.AddDays(-(int)date.DayOfWeek);
                    return GetEndDateTime(new DateTime(tempDt.Year, tempDt.Month, tempDt.Day, 0, 0, 0), resolution);

                case TimeFrame.Daily:
                    return GetEndDateTime(new DateTime(date.Year, date.Month, date.Day, 0, 0, 0), resolution);

                case TimeFrame.Hourly:
                    int i = 0;
                    while(i < 24)
                    {
                        if (date.Hour >= i && date.Hour < i + resolution.Size)
                            break;

                        i += resolution.Size;
                    }

                    return GetEndDateTime(new DateTime(date.Year, date.Month, date.Day, i, 0, 0), resolution);
                
                case TimeFrame.Minute:
                    int floor = 0;
                    int ceiling = resolution.Size;
                    while (ceiling < 60)
                    {
                        if (date.Minute >= floor && date.Minute < ceiling)
                            break;
                        floor += resolution.Size;
                        ceiling += resolution.Size;
                    }

                    return GetEndDateTime(new DateTime(date.Year, date.Month, date.Day, date.Hour, floor, 0), resolution);


                default:
                    return date;
            }

        }

        public static DateTime NormalizeAndGetStartDateTime(DateTime date, Resolution resolution)
        {
            switch (resolution.TimeFrame)
            {
                case TimeFrame.Yearly:
                    return new DateTime(date.Year, 01, 01).AddYears(1).AddMilliseconds(-1);

                case TimeFrame.Quarterly:
                    int month;
                    if (date.Month < 4)
                        month = 1;
                    else if (date.Month < 7)
                        month = 4;
                    else if (date.Month < 10)
                        month = 7;
                    else
                        month = 10;
                    return new DateTime(date.Year, month, 1).AddMonths(3);

                case TimeFrame.Monthly:
                    return new DateTime(date.Year, date.Month, 1, 0, 0, 0);

                case TimeFrame.Weekly:
                    var tempDt = date.AddDays(-(int)date.DayOfWeek);
                    tempDt = new DateTime(tempDt.Year, tempDt.Month, tempDt.Day, 0, 0, 0);
                    return tempDt;

                case TimeFrame.Daily:
                    return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0).AddDays(-(resolution.Size - 1));

                case TimeFrame.Hourly:
                    int i = 0;
                    while (i < 24)
                    {
                        if (date.Hour >= i && date.Hour < i + resolution.Size)
                            break;

                        i += resolution.Size;
                    }

                    return new DateTime(date.Year, date.Month, date.Day, i, 0, 0);

                case TimeFrame.Minute:
                    int floor = 0;
                    int ceiling = resolution.Size;
                    while (ceiling < 60)
                    {
                        if (date.Minute >= floor && date.Minute < ceiling)
                            break;
                        floor += resolution.Size;
                        ceiling += resolution.Size;
                    }

                    return new DateTime(date.Year, date.Month, date.Day, date.Hour, floor, 0);


                default:
                    return date;
            }

        }
        //public static List<Bar> CreateHourlyBarsFromMinuteBars(IEnumerable<Bar> minuteList, int hourlySize)
        //{
        //    var list = new List<Bar>();

        //    var fd = minuteList.First().DateTime;
        //    DateTime sd = new DateTime(fd.Year, fd.Month, fd.Day, 0, 0, 0);
        //    DateTime ed = sd.AddHours(hourlySize);

        //    while (sd <= minuteList.Last().EndDateTime)
        //    {
        //        var ls = minuteList.Where(p => p.DateTime >= sd && p.EndDateTime < ed);

        //        if (ls.Count() > 0)
        //        {
        //            var bar = composeBarFromList(sd, ed, ls);
        //            list.Add(bar);
        //        }

        //        sd = ed;
        //        ed = ed.AddHours(hourlySize);
        //    }

        //    return list;
        //}

        //public static List<Bar> CreateMonthlyBarsFromDailyBars(IEnumerable<Bar> dailyList, int monthlySize)
        //{
        //    var list = new List<Bar>();

        //    var fd = dailyList.First().DateTime;
        //    DateTime sd = new DateTime(fd.Year, 1, 1, 0, 0, 0);
        //    DateTime ed = sd.AddMonths(monthlySize);

        //    while (sd <= dailyList.Last().EndDateTime)
        //    {
        //        var ls = dailyList.Where(p => p.DateTime >= sd && p.EndDateTime < ed);

        //        if (ls.Count() > 0)
        //        {
        //            Bar bar = composeBarFromList(sd, ed, ls);
        //            list.Add(bar);
        //        }

        //        sd = ed;
        //        ed = ed.AddMonths(monthlySize);
        //    }

        //    return list;
        //}

        //public static List<Bar> CreateQuarterlyBarsFromMonthlyBars(IEnumerable<Bar> monthlyList)
        //{
        //    var list = new List<Bar>();

        //    var fd = monthlyList.First().DateTime;
        //    DateTime sd = new DateTime(fd.Year, 1, 1, 0, 0, 0);
        //    DateTime ed = sd.AddMonths(3);

        //    while (sd <= monthlyList.Last().EndDateTime)
        //    {
        //        var ls = monthlyList.Where(p => p.DateTime >= sd && p.EndDateTime < ed);

        //        if (ls.Count() > 0)
        //        {
        //            Bar bar = composeBarFromList(sd, ed, ls);
        //            list.Add(bar);
        //        }

        //        sd = ed;
        //        ed = ed.AddMonths(3);
        //    }

        //    return list;
        //}

        private static Bar composeBarFromList(DateTime sd, DateTime ed, IEnumerable<Bar> ls)
        {
            return new Bar { Open = ls.First().Open, High = ls.Max(p => p.High), Low = ls.Min(p => p.Low), Close = ls.Last().Close, Volume = ls.Sum(p => p.Volume), DateTime = sd};
        }
    }
}
