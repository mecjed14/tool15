using System.IO;

namespace Buhler.IoT.Environment.ChangeLogTool.Helper
{
    public interface IDirectoryService
    {
        DirectoryInfo EnsureDirectory(string path);
    }
}
