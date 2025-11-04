using System.Collections.Generic;
using System.Threading;

namespace CodeTest.Logger.Tests
{
    public class MockLogWriter : ILogWriter
    {
        public List<Log> Logs { get; } = new List<Log>();

        public MockLogWriter()
        {
        }
        
        public void WriteLog(Log log)
        {
            Logs.Add(log);
            Thread.Sleep(1);
        }
    }
}