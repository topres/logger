using System;

namespace CodeTest.Logger
{
    public interface IAsyncLogger
    {
        /// <summary>
        /// Subscribe to any exceptions that occurs in the processor.
        /// </summary>
        event Action<Exception> OnException;
        
        /// <summary>
        /// Stop the logging. Any pending logs that have not yet been logged to disk, will be lost.
        /// </summary>
        void StopWithoutFlush();

        /// <summary>
        /// Stop the logging. This method will block until all pending logs have been written to disk.
        /// </summary>
        void StopWithFlush();

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="content">The content to be written to the log</param>
        void WriteLog(string content);
    }
}
