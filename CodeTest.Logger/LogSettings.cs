namespace CodeTest.Logger
{
    public class LogSettings
    {
        public string LogPath { get; }
        
        /// <summary>
        /// Due to limitations in the .net core File API, buggy behavior can occur if you use multiple AsyncLogger
        /// instances on the same file. Each AsyncLogger instance should have it's own file name.
        /// This filename will be post-fixed with a timestamp.
        /// </summary>
        public string LogFileName { get; }
        
        public LogSettings(string logPath, string logFileName)
        {
            LogPath = logPath;
            LogFileName = logFileName;
        }
    }
}