using System;
using System.IO;
using System.Text;

namespace CodeTest.Logger
{
    /// <summary>
    /// This class is responsible for everything related to file IO
    /// </summary>
    public class FileLogWriter : ILogWriter
    {
        private readonly IClock _clock;
        private readonly string _logPath;
        private readonly string _logFileName;

        // the time which we last created a rolling log file
        private DateTime? _lastDateTime;
        
        private StreamWriter _writer;

        public FileLogWriter(IClock clock, string logPath, string logFileName)
        {
            _clock = clock;
            _logPath = logPath;
            _logFileName = logFileName;
        }
        
        public void WriteLog(Log log)
        {
            EnsureLogFileStreamIsCreated();
            
            var sb = new StringBuilder();
            sb.Append(log.Timestamp.ToString("yyyy-MM-dd HH:mm:ss:fff"));
            sb.Append("\t");
            sb.Append(log.Text);
            sb.Append("\t");
            
            _writer.WriteLine(sb.ToString());
        }

        private void EnsureLogFileStreamIsCreated()
        {
            var currentDateTime = _clock.CurrentDanishTime();

            if (_lastDateTime == null || currentDateTime.Day != _lastDateTime.Value.Day)
            {
                var fileName = ToLogFileName(currentDateTime);
                _writer = LogFileStreamCreator.LogFileStream(_logPath, fileName);
                _writer.AutoFlush = true;
                _lastDateTime = currentDateTime;
            }
        }
        
        private string ToLogFileName(DateTime dateTime)
        {
            var timestamp = dateTime.ToString("yyyyMMdd");
            return $"{_logFileName}-{timestamp}.log";
        } 
    }
}