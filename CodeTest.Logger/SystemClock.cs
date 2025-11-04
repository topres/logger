using System;

namespace CodeTest.Logger
{
    public class SystemClock : IClock
    {
        DateTime IClock.CurrentDanishTime()
        {
            return CurrentDanishTime();
        }
        
        public static DateTime CurrentDanishTime()
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Copenhagen");
            var utc = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
            return TimeZoneInfo.ConvertTime(utc, TimeZoneInfo.Utc, timeZone);
        }
    }
}