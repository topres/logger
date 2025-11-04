using System;

namespace CodeTest.Logger
{
    public class Log
    {
        public DateTime Timestamp { get; }
        public string Text { get; }
        
        public Log(DateTime timestamp, string text)
        {
            Timestamp = timestamp;
            Text = text;
        }
    }
}