using System.IO;
using System.Threading;

namespace CodeTest.Logger
{
    public static class LogFileStreamCreator
    {
        public static string Header = "Timestamp".PadRight(25, ' ') + "\t" + "Data".PadRight(15, ' ') + "\t";

        public static StreamWriter LogFileStream(string path, string fileName)
        {
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            var filePath = Path.Combine(path, fileName);

            EnsureFileExists(filePath);
            WriteHeaderIfFirstLineIsEmpty(filePath);
            return File.AppendText(filePath);
        }

        private static void EnsureFileExists(string filePath)
        {
            using (_ = File.AppendText(filePath))
            {
            }
        }

        private static void WriteHeaderIfFirstLineIsEmpty(string filePath)
        {
            using (var fileStream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            using (var reader = new StreamReader(fileStream))
            using (var writer = new StreamWriter(fileStream))
            {
                var firstLine = reader.ReadLine();

                if (string.IsNullOrWhiteSpace(firstLine))
                {
                    writer.WriteLine(Header);
                }
            }
        }
    }
}