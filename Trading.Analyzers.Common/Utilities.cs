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
                    //int top = bottom + resolution.Size;

                    //if (date.Hour >= bottom)
                    //{
                    //    return dt.AddHours(-(date.Hour - bottom));
                    //}

                    //top = 24 - top;

                    //if (date.Hour < top)
                    //{
                    //    return dt.AddHours(-(resolution.Size - date.Hour));
                    //}
                    //bottom = top;
                    //top += resolution.Size;
                    //*********************************
                    //int adjustant = date.IsDaylightSavingTime() ? 21 : 22;
                    //DateTime dt = new DateTime(date.Year, date.Month, date.Day, date.Hour, 0, 0).AddHours(-adjustant);

                    //int bottom = 0;
                    //int top = resolution.Size;
                    //while(top <= 24)
                    //{
                    //    if (dt.Hour >= bottom && dt.Hour < top)
                    //        break;
                    //    bottom += resolution.Size;
                    //    top += resolution.Size;
                    //}
                    //dt = new DateTime(dt.Year, dt.Month, dt.Day, bottom, dt.Minute, dt.Second);
                    //dt = dt.AddHours(adjustant);
                    //return dt;

                    int cutOffHour = date.IsDaylightSavingTime() ? 21 : 22;
                    DateTime startDate = new DateTime(date.Year, date.Month, date.Day, cutOffHour, 0, 0).AddDays(-1);
                    DateTime endDate = startDate.AddHours(resolution.Size);
                    for(int i = 0; i < 24/resolution.Size+4; i++)
                    {
                        if (date >= startDate && date < endDate)
                            break;

                        startDate = endDate;
                        endDate = endDate.AddHours(resolution.Size);
                    }
                    return startDate;
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
