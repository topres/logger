using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;

namespace CodeTest.Logger
{
    public class AsyncLogger : IAsyncLogger
    {
        private readonly ConcurrentQueue<Log> _pendingLogs = new ConcurrentQueue<Log>();
        private readonly ILogWriter _logWriter;
        private readonly Thread _processorThread;

        private bool _shouldExit;
        private bool _shouldExitWithFlush;

        public event Action<Exception>? OnException;
        
        private AsyncLogger(ILogWriter logWriter)
        {
            _logWriter = logWriter;
            _processorThread = new Thread(() => ExceptionHandler(PendingLogProcessor));
            _processorThread.Start();
        }
        
        public static AsyncLogger Initialize(ILogWriter logWriter) => new AsyncLogger(logWriter);

        public static AsyncLogger Initialize(LogSettings settings)
        {
            var logWriter = new FileLogWriter(new SystemClock(), settings.LogPath, settings.LogFileName);
            return new AsyncLogger(logWriter);
        }

        public void StopWithoutFlush() => _shouldExit = true;
        
        public void StopWithFlush()
        {
            _shouldExitWithFlush = true;
            
            // joining the processorThread with the calling thread will block until the processing is done.
            _processorThread.Join();
        }

        public void WriteLog(string content)
        {
            if (_shouldExit || _shouldExitWithFlush)
            {
                throw new InvalidOperationException("This logger has been stopped, you cannot write logs at this time.");
            }
            
            var log = new Log(SystemClock.CurrentDanishTime(), content);
            _pendingLogs.Enqueue(log);
        }
        
        private void ExceptionHandler(Action processor)
        {
            try
            {
                processor();
            }
            catch (Exception e)
            {
                OnException?.Invoke(e);
            }
        }
        
        private void PendingLogProcessor()
        {
            void WriteNextLog()
            {
                if(_pendingLogs.TryDequeue(out var log))
                {
                    _logWriter.WriteLog(log);
                }
            }
            
            while (_shouldExit == false && _shouldExitWithFlush == false)
            {
                if (_pendingLogs.Count > 0)
                {
                    WriteNextLog();
                }
            }

            if (_shouldExitWithFlush)
            {
                while (_pendingLogs.Count > 0)
                {
                    WriteNextLog();
                }
            }
        }
    }
}