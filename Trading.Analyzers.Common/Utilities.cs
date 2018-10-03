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
                    return date.AddMonths(1);
                case TimeFrame.Weekly:
                    return date.AddDays(7);
                case TimeFrame.Daily:
                    return new DateTime(date.Year, date.Month, date.Day).AddDays(1).AddMilliseconds(-1);
                case TimeFrame.Hourly:
                    //int portionLengh = 24 / resolution.Size;
                    //int startHour = 21;
                    //int endHour = 0;
                    //DateTime dt = date;
                    //for(int i = 1; i <= portionLengh; i++)
                    //{
                    //    endHour += startHour + (portionLengh * i);
                    //    if(endHour > 24)

                    //}
                    DateTime dt = new DateTime(date.Year, date.Month, date.Day, date.Hour, 0, 0);
                    
                    int bottom = 21;
                    int top = bottom + resolution.Size;
                    for(int i = 1; i <= 24/resolution.Size; i++)
                    {

                        if (date.Hour >= bottom && date.Hour < top)
                        {
                            dt = dt.AddHours(-(date.Hour - bottom));
                            break;
                        }
                        if(top >= 24)
                        {
                            bottom = top - 24;
                            top = bottom + resolution.Size;
                            continue;
                        }
                        bottom += resolution.Size;
                        top += resolution.Size;
                    }
                    return dt;

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
    }
}
