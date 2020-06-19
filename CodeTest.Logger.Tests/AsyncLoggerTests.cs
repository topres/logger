using System;
using System.IO;
using System.Threading;
using Moq;
using Xunit;

namespace CodeTest.Logger.Tests
{
    public class AsyncLoggerTests
    {
        [Fact]
        public void WriteLog_will_result_in_files_being_written_to_disk()
        {
            var dir = "tmp/logs/c13d3e35-e4b8-4ae7-ac97-06bfd1d8f275";
            
            FileSystemHelper.EmptyDirectory(dir);

            var logger = AsyncLogger.Initialize(new LogSettings(dir, "log"));
            
            logger.WriteLog("bar");
            logger.WriteLog("baz");
            logger.StopWithFlush();

            var fileName = "log-" + SystemClock.CurrentDanishTime().ToString("yyyyMMdd") + ".log";
            
            var lines = File.ReadAllLines(Path.Combine(dir, fileName));
            
            // Expecting 3 lines, the header and the two logs we wrote
            Assert.Equal(3, lines.Length);
            
            // Check that header looks correct
            Assert.StartsWith("Timestamp", lines[0]);
            Assert.Contains("Data", lines[0]);
            
            // check for the two logs
            Assert.Contains("bar", lines[1]);
            Assert.Contains("baz", lines[2]);
        }
        
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void WriteLog_InvalidOperation_exception_if_WriteLog_is_called_after_calling_stop(bool flush)
        {
            var logger = AsyncLogger.Initialize(new LogSettings("tmp/path", "log"));
            
            if (flush)
            {
                logger.StopWithFlush();
            }
            else
            {
                logger.StopWithoutFlush();
            }

            Assert.Throws<InvalidOperationException>(() =>
            {
                logger.WriteLog("foo");
            });
        }

        [Fact]
        public void StopWithoutFlush_will_stop_processing_logs()
        {
            var logWriter = new MockLogWriter();

            var logger = AsyncLogger.Initialize(logWriter);

            for (int i = 0; i < 50; i++)
            {
                logger.WriteLog("Log" + 1);
            }
            
            // now we stop the processing, without flushing
            logger.StopWithoutFlush();
            
            // sleep for a while then verify that not all logs have been flushed
            Thread.Sleep(100);

            int logCount = logWriter.Logs.Count;
            
            Assert.True(
                logCount < 25,
                $"Expected less than {logCount} logs to be written " +
                $"when the logger was stopped without flushing. Actual logged: {logCount}");
        }
        
        [Fact]
        public void StopWithFlush_will_block_current_thread_until_all_messages_have_been_processed()
        {
            var logWriter = new MockLogWriter();
            var logger = AsyncLogger.Initialize(logWriter);

            for (int i = 0; i < 20; i++)
            {
                logger.WriteLog("Log" + 1);
            }
            
            logger.StopWithFlush();
            
            Assert.Equal(20, logWriter.Logs.Count);
        }
    }
}