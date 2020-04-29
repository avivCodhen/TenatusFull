using System;

namespace Tenatus.API.Extensions
{
    public static class DateExtensions
    {
        public static string FormatDate(this DateTimeOffset s)
        {
            return $"{s:MMM d, yyyy}";
        }

        public static string FormatDateTime(this DateTimeOffset s)
        {
            return $"{s:MMM d, yyyy, hh:mm tt}";
        }

        public static string MonthAndDay(this DateTimeOffset s)
        {
            return $"{s:MMM d}";
        }
    }
}