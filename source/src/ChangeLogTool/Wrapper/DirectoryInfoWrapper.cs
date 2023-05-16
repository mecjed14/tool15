using Buhler.IoT.Environment.ChangeLogTool.Config;
using System.Diagnostics.CodeAnalysis;
using System.IO;


namespace Buhler.IoT.Environment.ChangeLogTool.Wrapper
{
    [ExcludeFromCodeCoverage]
    public class DirectoryInfoWrapper : IDirectoryInfoWrapper
    {
        private readonly DirectoryInfo _directoryInfo;

        public DirectoryInfoWrapper(IAppSettings appSettings)
        {
            _directoryInfo = new DirectoryInfo(appSettings.UnreleasedChangeLogFolderFullPath);
        }

        public FileInfo[] GetFiles(DirectoryInfo directoryInfo,string searchPatterns)
        {
            return _directoryInfo.GetFiles(searchPatterns);
        }

        public DirectoryInfo GetDirectoryInfo()
        {
            return new DirectoryInfo (_directoryInfo.FullName);
        }

        public FileInfo[] GeFiles(string serachPattern)
        {
            return _directoryInfo.GetFiles(serachPattern);
        }

        public DirectoryInfo CreatSubDirectory(string path)
        {
            return _directoryInfo.CreateSubdirectory(path);
        }
    }
}

