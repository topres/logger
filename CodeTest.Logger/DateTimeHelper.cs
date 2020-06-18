using System;

namespace CodeTest.Logger
{
    public static class DateTimeHelper
    {
        public static DateTime CurrentDanishTime()
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Copenhagen");
            var utc = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
            return TimeZoneInfo.ConvertTime(utc, TimeZoneInfo.Utc, timeZone);
        }

    }
}