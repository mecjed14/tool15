using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;


namespace Buhler.IoT.Environment.ChangeLogTool.Wrapper
{
    [ExcludeFromCodeCoverage]
    public class FileWrapper : IFileWrapper
    {
        public StreamWriter CreateFile(string path)
        {
            return File.CreateText(path);
        }

        public string[] ReadAllLines(string path)
        {
            return File.ReadAllLines(path);
        }

        public string ReadAllText(string path) 
        {
            return File.ReadAllText(path);  
        }

        public void WriteAllLines(string changeLogFilePath, IEnumerable<string> masterChangeLogContent)
        {
            File.WriteAllLines(changeLogFilePath, masterChangeLogContent);  
        }

        public void DeleteFile (string path)
        {
            File.Delete(path);
        }
    }
}
