using System;
using System.Threading.Tasks;
using CodeTest.Logger;

namespace CodeTest.Application
{
    class Program
    {
        static async Task Main()
        {
            LogWithFlushing();
            await LogWithoutFlushing();

            Console.WriteLine("Terminated.");
        }

        private static void LogWithFlushing()
        {
            var settings = new LogSettings(logPath: $"tmp/logs", logFileName: "log-with-flushing");
            IAsyncLogger logger = AsyncLogger.Initialize(settings);

            logger.OnException += ex => Console.WriteLine("Logger exception: " + ex.Message);

            for (int i = 0; i < 15; i++)
            {
                logger.WriteLog("Number with Flush: " + i);
            }

            logger.StopWithFlush();
        }

        private static async Task LogWithoutFlushing()
        {
            var settings = new LogSettings(logPath: $"tmp/logs", logFileName: "log-without-flushing");
            IAsyncLogger logger = AsyncLogger.Initialize(settings);
            
            logger.OnException += ex => Console.WriteLine("Logger exception: " + ex.Message);

            for (int i = 50; i > 0; i--)
            {
                logger.WriteLog("Number with No flush: " + i);
                await Task.Delay(5);
            }

            logger.StopWithoutFlush();
        }
    }
}