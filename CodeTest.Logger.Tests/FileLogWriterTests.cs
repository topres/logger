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

            var logPath = "tmp/logs/tests/rolling_log_file_tests";
            FileSystemHelper.EmptyDirectory(logPath);
            var logName = "log";
            var writer = new FileLogWriter(clock.Object, logPath, logName);

            DateTime SetupCurrentTime(int month, int day)
            {
                var dateTime = new DateTime(2020, month, day, 12, 0, 0);
                clock.Setup(e => e.CurrentDanishTime()).Returns(dateTime);
                return dateTime;
            }

            void AssertWrittenLogsWithNoInitialLogFile()
            {
                var time = SetupCurrentTime(01, 01);
                writer.WriteLog(Log("log1"));
                writer.WriteLog(Log("log2"));
                var lines = ReadFile(logPath, logName, time);
                Assert.Equal(3, lines.Length);
            }

            void AssertWritenLogsWithInitialLogFileForNextDay()
            {
                var time = SetupCurrentTime(01, 02);
                writer.WriteLog(Log("log3"));
                var lines = ReadFile(logPath, logName, time);
                Assert.Equal(2, lines.Length);
            }
            
            void AssertWrittenLogsWithIncrementedMonthAndDecrementedDay()
            {
                var time = SetupCurrentTime(02, 01);
                writer.WriteLog(Log("log4"));
                var lines = ReadFile(logPath, logName, time);
                Assert.Equal(2, lines.Length);
            }
            
            AssertWrittenLogsWithNoInitialLogFile();
            AssertWritenLogsWithInitialLogFileForNextDay();
            AssertWrittenLogsWithIncrementedMonthAndDecrementedDay();
        }
        
        [Fact]
        public void WriteLog_will_result_in_files_being_written_correctly_to_disk()
        {
            var currentTime = new DateTime(2020, 02, 01, 12, 0, 0);
            var clock = new Mock<IClock>();
            clock.Setup(e => e.CurrentDanishTime()).Returns(currentTime);
            
            var logPath = "tmp/logs/tests/can_write_files_to_disk";
            FileSystemHelper.EmptyDirectory(logPath);
            var logName = "log";
            
            var writer = new FileLogWriter(clock.Object, logPath, logName);
            
            var log1 = new Log(new DateTime(2020, 01, 01, 12, 0, 10), "log1");
            var log2 = new Log(new DateTime(2020, 01, 01, 12, 0, 11), "log2");
            
            writer.WriteLog(log1);
            writer.WriteLog(log2);
            
            var lines = ReadFile(logPath, logName, currentTime);
            
            // Expecting 3 lines, the header and the two logs we wrote
            Assert.Equal(3, lines.Length);
            
            // Check that header looks correct
            Assert.Equal(LogFileStreamCreator.Header, lines[0]);

            string ExpectedLogFormat(Log log) => $"{log.Timestamp:yyyy-MM-dd HH:mm:ss:fff}\t{log.Text}\t";
            
            // check for the two logs
            Assert.Equal(ExpectedLogFormat(log1), lines[1]);
            Assert.Equal(ExpectedLogFormat(log2), lines[2]);
        }

        string[] ReadFile(string logPath, string logName, DateTime dateTime)
        {
            var fileName = $"{logPath}/{logName}-{dateTime:yyyyMMdd}.log";
            return File.ReadAllLines(fileName);
        }

        private Log Log(string text)
        {
            return new Log(SystemClock.CurrentDanishTime(), text);
        }
    }
}