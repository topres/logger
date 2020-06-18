using System;

namespace CodeTest.Logger
{
    public class SystemClock : IClock
    {
        public DateTime CurrentDanishTime()
        {
            return DateTimeHelper.CurrentDanishTime();
        }
    }
}