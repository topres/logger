using System.IO;

namespace CodeTest.Logger.Tests
{
    public static class FileSystemHelper
    {
        public static void EmptyDirectory(string dir)
        {
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }
            
            Directory.CreateDirectory(dir);
        } 
    }
}