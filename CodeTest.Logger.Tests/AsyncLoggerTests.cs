using System;
using System.IO;
using System.Threading;
using Moq;
using Xunit;

namespace CodeTest.Logger.Tests
{
    public class AsyncLoggerTests
    {
        public AsyncLoggerTests()
        {
        }
        
        
        [Fact]
        public void WriteLog_will_result_in_files_being_written_to_disk()
        {
            var dir = "tmp/logs/c13d3e35-e4b8-4ae7-ac97-06bfd1d8f275";
            
            CleanDirectory(dir);

            var logger = AsyncLogger.Initialize(new LogSettings(dir, "log"));
            
            logger.WriteLog("bar");
            logger.WriteLog("baz");
            logger.StopWithFlush();

            var fileName = "log-" + DateTimeHelper.CurrentDanishTime().ToString("yyyyMMdd") + ".log";
            
            var lines = File.ReadAllLines(Path.Combine(dir, fileName));
            Assert.Equal(3, lines.Length);
            
            // check for header
            Assert.Contains("Timestamp", lines[0]);
            
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
            var logWriter = new Mock<ILogWriter>();
            var logger = new AsyncLogger(logWriter.Object);

            for (int i = 0; i < 20; i++)
            {
                logger.WriteLog("Log" + 1);
            }
            
            // initially the logger will sleep 50ms, we give it an extra 25ms to start processing, so 75ms in total
            Thread.Sleep(75);
            
            // now we stop the processing, without flushing
            logger.StopWithoutFlush();
            
            // sleep for a while then verify that still only one batch was processed
            Thread.Sleep(500);
            logWriter.Verify(e => e.WriteLog(It.IsAny<Log>()), Times.Exactly(5));
        }
        
        [Fact]
        public void StopWithFlush_will_block_current_thread_until_all_messages_have_been_processed()
        {
            var logWriter = new Mock<ILogWriter>();
            var logger = new AsyncLogger(logWriter.Object);

            for (int i = 0; i < 100; i++)
            {
                logger.WriteLog("Log" + 1);
            }
            
            logger.StopWithFlush();
            
            logWriter.Verify(e => e.WriteLog(It.IsAny<Log>()), Times.Exactly(100));
        }

        private void CleanDirectory(string dir)
        {
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }
            
            Directory.CreateDirectory(dir);
        }
    }
}