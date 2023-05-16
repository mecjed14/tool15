using Buhler.IoT.Environment.ChangeLogTool.ChangeLog;
using Buhler.IoT.Environment.ChangeLogTool.Config;
using Buhler.IoT.Environment.ChangeLogTool.Wrapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Buhler.IoT.Environment.ChangeLogTool.Helper
{
    public class FileHelper : IFileHelper
    {
        private readonly IAppSettings _settings;
        private readonly IConsoleHelper _consoleHelper;
        private readonly IFileWrapper _fileWrapper;
        private readonly IDirectoryInfoWrapper _directoryInfoWrapper;


        public FileHelper(IAppSettings settings, IConsoleHelper consoleHelper,IFileWrapper fileWrapper,IDirectoryInfoWrapper directoryInfoWrapper)
        {
            _settings = settings;
            _consoleHelper = consoleHelper;
            _fileWrapper = fileWrapper;
            _directoryInfoWrapper = directoryInfoWrapper;
        }

        public void WriteObjectToJson<T>(T obj, string path) where T : class
        {
            var fileInfo = new FileInfo(path);
            if (fileInfo.Directory?.Exists != true)
            {
                fileInfo.Directory?.Create();
            }

            using var sw = _fileWrapper.CreateFile(fileInfo.FullName);
            var fileContent = JsonConvert.SerializeObject(obj, Formatting.Indented);
            sw.WriteLine(fileContent);
        }

        public FileInfo[] GetAllChangelogFiles(DirectoryInfo unreleasedDirInfo)
        {
            return _directoryInfoWrapper.GetFiles(unreleasedDirInfo,"*.json");
        }

        public string[] ReadAllLineOfTheFiles(string filePath)
        {
            return _fileWrapper.ReadAllLines(filePath);
        }

        public string GetFileContent(string path)
        {
            if (path != null)
            {
                return _fileWrapper.ReadAllText(path);
            }
            return null;
        }

        public void WriteAllLinesOfTheFiles(string changeLogFilePath, IList<string> masterChangeLogContent)
        {
            _fileWrapper.WriteAllLines(changeLogFilePath, masterChangeLogContent);
        }

        public void DeleteTheFile(string path)
        {
           _fileWrapper.DeleteFile(path);
        }

        public void WriteChangeLogEntry(ChangeLogEntry changeLogEntry)
        {
            ArgumentNullException.ThrowIfNull(changeLogEntry);
            
             var changelogFileName = Path.Combine(_settings.UnreleasedChangeLogFolderFullPath, $"{DateTime.UtcNow.ToFileTime()}.json");
            _consoleHelper.LogFormattedMessage("A changelog json file with the following message will be created: {0}", changeLogEntry.FullChangeLogMessage);
            WriteObjectToJson(changeLogEntry, changelogFileName);
        }
    }
}


