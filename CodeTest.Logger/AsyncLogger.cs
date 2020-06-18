using System;
using System.Collections.Concurrent;
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

        private bool ShouldProcess => _shouldExit == false && _shouldExitWithFlush == false;
        
        public AsyncLogger(ILogWriter logWriter)
        {
            _logWriter = logWriter;
            _processorThread = new Thread(PendingLogProcessor);
            _processorThread.Start();
        }

        public static AsyncLogger Initialize(LogSettings settings)
        {
            var logWriter = new FileLogWriter(new SystemClock(), settings.LogPath, settings.LogFileName);
            return new AsyncLogger(logWriter);
        }

        private void PendingLogProcessor()
        {
            while (ShouldProcess)
            {
                while (_pendingLogs.Count > 0 && _shouldExit == false)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        if(_pendingLogs.TryDequeue(out var log))
                        {
                            _logWriter.WriteLog(log);
                            Console.WriteLine("log: " + log.Text);
                        }
                    }

                    Thread.Sleep(50);
                }
            }
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
            
            var log = new Log(DateTimeHelper.CurrentDanishTime(), content);
            _pendingLogs.Enqueue(log);
        }
    }
}