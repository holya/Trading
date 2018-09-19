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
            //DateTime dt = DateTime.Now;
            switch (resolution.TimeFrame)
            {
                case TimeFrame.Yearly:
                    return new DateTime(date.Year, 01, 01, 00, 00, 00, date.Kind);
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
                    return new DateTime(date.Year, month, 01, 00, 00, 00, date.Kind);
                case TimeFrame.Monthly:
                    return new DateTime(date.Year, date.Month, 01, 00, 00, 00, date.Kind);
                case TimeFrame.Weekly:
                    var tempDate = date.AddDays(-((int)date.DayOfWeek));
                    return new DateTime(tempDate.Year, tempDate.Month, tempDate.Day, 00, 00, 00, date.Kind);
                case TimeFrame.Daily:
                    return new DateTime(date.Year, date.Month, date.Day, 00, 00, 00, date.Kind);
                case TimeFrame.Hourly:
                    int bottom = 0;
                    int top = resolution.Size;
                    while(top < 24)
                    {
                        if (date.Hour >= bottom && date.Hour < top)
                            break;
                        bottom += resolution.Size;
                        top += resolution.Size;
                    }
                    if (top >= 24)
                        return new DateTime(date.Year, date.Month, date.Day, bottom,  00, 00, 00, date.Kind).AddHours(resolution.Size).AddMilliseconds(-1);

                    return new DateTime(date.Year, date.Month, date.Day, top, 00, 00, 00, date.Kind);
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

                    if(ceiling >= 60)
                        return new DateTime(date.Year, date.Month, date.Day, date.Hour,            floor, 00, 00, date.Kind).AddMinutes       (resolution.Size).AddMilliseconds(-1);

                    //int minute = 0;
                    //int maxLimit = 60;
                    //int segments = 0;

                    //segments = maxLimit / resolution.Size;
                    //int[] partitions = new int[segments + 1];

                    //for (int i = 0; i <= segments; i++)
                    //{
                    //    partitions[i] = i * segments;
                    //}

                    //foreach (int i in partitions)
                    //{
                    //    int index = date.Minute / i;
                    //    minute = index;
                    //}

                    return new DateTime(date.Year, date.Month, date.Day, date.Hour, ceiling, 00, 00, date.Kind);

                default:
                    return DateTime.UtcNow;
            }
        }
    }
}
