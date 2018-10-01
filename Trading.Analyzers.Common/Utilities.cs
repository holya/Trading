using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;
namespace Trading.Analyzers.Common
{
    public static class Utilities
    {
        public static DateTime NormalizeBarDateTime_FXCM(DateTime date, Resolution resolution)
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
                    return new DateTime(date.Year, date.Month, 01).AddMonths(resolution.Size).AddMilliseconds(-1);
                case TimeFrame.Weekly:
                    var tempDate = date.AddDays(-(int)date.DayOfWeek);
                    return new DateTime(tempDate.Year, tempDate.Month, tempDate.Day).AddDays(7).AddMilliseconds(-1);
                case TimeFrame.Daily:
                    return new DateTime(date.Year, date.Month, date.Day).AddDays(1).AddMilliseconds(-1);
                case TimeFrame.Hourly:
                    int bottom = 9;
                    int top = resolution.Size;
                    while(top < 24)
                    {
                        if (date.Hour >= bottom && date.Hour < top)
                            break;
                        bottom += resolution.Size;
                        top += resolution.Size;
                    }
                    return new DateTime(date.Year, date.Month, date.Day, bottom, 0, 0).AddHours(resolution.Size).AddMilliseconds(-1);

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
                    return new DateTime(date.Year, date.Month, date.Day, date.Hour, floor, 0).AddMinutes(resolution.Size).AddMilliseconds(-1);

                default:
                    return date;
            }
        }
    }
}
