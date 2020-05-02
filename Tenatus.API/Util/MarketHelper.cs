using System;

namespace Tenatus.API.Util
{
    public class MarketHelper
    {
        public static (DateTime open, DateTime close) GetOpenAndClose()
        {
            var open = Convert.ToDateTime("9:30");
            var close = Convert.ToDateTime("16:00");
            return (open, close);
        }

        public static DateTime GetEasternTime()
        {
            var timeUtc = DateTime.UtcNow;
            var easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            var easternTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, easternZone);
            return easternTime;
        }
        
        public static bool IsMarketOpen()
        {
            if (DateTimeOffset.Now.DayOfWeek == DayOfWeek.Saturday || DateTimeOffset.Now.DayOfWeek == DayOfWeek.Sunday)
                return false;

            var easternTime = GetEasternTime();
            var (open, close) = GetOpenAndClose();
            return easternTime >= open && close > easternTime;
        }
    }
}