using System;
using System.IO;
using Moq;
using Xunit;

namespace CodeTest.Logger.Tests
{
    public class FileLogWriterTests
    {

        [Fact]
        public void Verify_that_rollings_log_files_are_created_every_day()
        {
            var clock = new Mock<IClock>();

            var logPath = "tmp/logs/9a8bb8d0-66ac-4b79-b683-5f3497ae9ba1";
            var logName = "log";
            var writer = new FileLogWriter(clock.Object, logPath, logName);
            
            // first log file
            var dateTime1 = new DateTime(2020, 01, 01, 12, 0, 0);
            clock.Setup(e => e.CurrentDanishTime()).Returns(dateTime1);
            writer.WriteLog(Log("log1"));
            writer.WriteLog(Log("log2"));
            var lines1 = ReadFile(logPath, logName, dateTime1);
            Assert.Equal(3, lines1.Length);
            
            // second log file, increment day
            var dateTime2 = new DateTime(2020, 01, 02, 12, 0, 0);
            clock.Setup(e => e.CurrentDanishTime()).Returns(dateTime2);
            writer.WriteLog(Log("log3"));
            var lines2 = ReadFile(logPath, logName, dateTime2);
            Assert.Equal(2, lines2.Length);
            
            // third log file, increment month decrement day, to ensure it doesn't only look at DateTime.Day
            var dateTime3 = new DateTime(2020, 02, 01, 12, 0, 0);
            clock.Setup(e => e.CurrentDanishTime()).Returns(dateTime3);
            writer.WriteLog(Log("log4"));
            var lines3 = ReadFile(logPath, logName, dateTime3);
            Assert.Equal(2, lines3.Length);
        }

        string[] ReadFile(string logPath, string logName, DateTime dateTime)
        {
            var fileName = $"{logPath}/{logName}-{dateTime:yyyyMMdd}.log";
            return File.ReadAllLines(fileName);
        }

        private Log Log(string text)
        {
            return new Log(DateTimeHelper.CurrentDanishTime(), text);
        }
    }
}