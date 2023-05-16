using System.Collections.Generic;
using System.IO;


namespace Buhler.IoT.Environment.ChangeLogTool.Wrapper
{
    public interface IFileWrapper
    {
        StreamWriter CreateFile(string path);
        string[] ReadAllLines(string path);
        string ReadAllText(string path);
        void WriteAllLines(string changeLogFilePath, IEnumerable<string> masterChangeLogContent);
        void DeleteFile(string path);
    }
}
