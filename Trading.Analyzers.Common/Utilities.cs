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
            DateTime dt = DateTime.Now;
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
                    int hour = 0;
                    int bottom = 0;
                    int top = resolution.Size;
                    while(top <= 24)
                    {
                        if (date.Hour >= bottom && date.Hour < top)
                        {
                            hour = bottom;
                            break;
                        }
                        bottom += resolution.Size;
                        top += resolution.Size;
                    }
                    return new DateTime(date.Year, date.Month, date.Day, hour, 00, 00, 00, date.Kind);
                case TimeFrame.Minute:
                    int minute = 0;
                    int floor = 0;
                    int ceiling = resolution.Size;
                    while (ceiling <= 60)
                    {
                        if (date.Minute >= floor && date.Minute < ceiling)
                        {
                            minute = floor;
                            break;
                        }
                        floor += resolution.Size;
                        ceiling += resolution.Size;
                    }
                    return new DateTime(date.Year, date.Month, date.Day, date.Hour, minute, 00, 00, date.Kind);

                default:
                    break;
            }
            return dt;
        }
    }
}
