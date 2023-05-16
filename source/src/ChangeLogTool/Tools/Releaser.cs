using Buhler.IoT.Environment.ChangeLogTool.ChangeLog;
using Buhler.IoT.Environment.ChangeLogTool.CommandLineOptions;
using Buhler.IoT.Environment.ChangeLogTool.Config;
using Buhler.IoT.Environment.ChangeLogTool.Exceptions;
using Buhler.IoT.Environment.ChangeLogTool.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics; //NOSONAR
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;


namespace Buhler.IoT.Environment.ChangeLogTool.Tools
{
    public class Releaser : IReleaser
    {
        private readonly IAppSettings _settings;
        private readonly IConsoleHelper _consoleHelper;
        private readonly IDirectoryService _directoryService;
        private readonly IFileHelper _fileHelper;
        private readonly IReleaseManager _releaseManager;

        public Releaser(IConsoleHelper consoleHelper, IDirectoryService directoryService, IFileHelper fileHelper, IReleaseManager releaseManagerHelper,IAppSettings settings)
        {
            _consoleHelper = consoleHelper;
            _directoryService = directoryService;
            _fileHelper = fileHelper;
            _releaseManager = releaseManagerHelper;
            _settings = settings;
        }

        public int ParseAndExecuteGenerateReleaseOptions(GenerateReleaseOptions options)
        {
            var releaseCheckResult = _releaseManager.CheckTheRelease(options);

            if( releaseCheckResult != 0) 
            {
                return releaseCheckResult;
            }
            var unreleasedDirInfo = _directoryService.EnsureDirectory(_settings.UnreleasedChangeLogFolderFullPath);

            // Get all json files under the unchanged directory.
            var unreleasedChangeLogFiles = _fileHelper.GetAllChangelogFiles(unreleasedDirInfo);
            if (!unreleasedChangeLogFiles.Any())
            {
                _consoleHelper.LogMessage($"There are no *.json files in the directory {_settings.UnreleasedChangeLogFolderFullPath} to process.");
#if DEBUG
                if (!Debugger.IsAttached)
                {
#endif
                    return 3;
#if DEBUG
                }
#endif
            }

            Version latestVersionInMasterChangelog = null;

            var masterChangeLogContent = _fileHelper.ReadAllLineOfTheFiles(_settings.MasterChangeLogFileFullPath).ToList();

            // Search for the first occurrence of the latest release version (e.g ## [v1.3.4] - 2020-01-01)
            var latestReleaseVersionRegex = new Regex(@"^\#\#\s*\[v([\d]+)\.([\d]+)\.([\d]+)\]\s*.\s([\d]{4}\-[\d]{1,2}\-[\d]{1,2})");
            var insertNewChangeLogEntryIndex = -1;

            var firstMatch = masterChangeLogContent.Select((line, index) => new { Match = latestReleaseVersionRegex.Match(line), Index = index }).FirstOrDefault(o => o.Match.Success);

            if (firstMatch != null)
            {
                var major = firstMatch.Match.Groups[1].Value;
                var minor = firstMatch.Match.Groups[2].Value;
                var build = firstMatch.Match.Groups[3].Value;
                latestVersionInMasterChangelog = VersionHelper.ParseVersion(major, minor, build);
                insertNewChangeLogEntryIndex = firstMatch.Index - 1;
            }

            var isFirstEntry = latestVersionInMasterChangelog == null;

            
            var releaseVersion = GetReleaseVersion(options, isFirstEntry, latestVersionInMasterChangelog);

            if (!isFirstEntry && insertNewChangeLogEntryIndex < 1)
            {
                _consoleHelper.LogMessage($"No latest version found in file {_settings.MasterChangeLogFileFullPath}, therefore the changelog entries cannot be added...");
                return 4;
            }

            var releasedChangeLogEntries = GetChangeLogsFromFiles(options.HotFix, unreleasedChangeLogFiles, releaseVersion);

            // Insert all changelog entries
            foreach (var entry in releasedChangeLogEntries)
            {
                var insertIndex = isFirstEntry ? masterChangeLogContent.Count : insertNewChangeLogEntryIndex;
                masterChangeLogContent.Insert(insertIndex, entry);
                insertNewChangeLogEntryIndex++;
            }

            // Write the release change log file
            _fileHelper.WriteAllLinesOfTheFiles(_settings.MasterChangeLogFileFullPath, masterChangeLogContent);
            _releaseManager.BranchCreator(options, releaseVersion, _settings);
            return 0;
        }

        private static Version GetReleaseVersion(GenerateReleaseOptions options, bool isFirstEntry, Version latestVersionInMasterChangelog)
        {
            var releaseVersion = new Version();
            if (isFirstEntry)
            {
                releaseVersion = new Version(1, 0, 0);
            }
            else if (options.Major)
            {
                var majorVersionNumber = CheckTheVersioningNumber(latestVersionInMasterChangelog.Major,options);
                return new Version(majorVersionNumber, 0, 0);
            }

            else if (options.Minor)
            {
                var minorVersionNumber = CheckTheVersioningNumber(latestVersionInMasterChangelog.Minor,options);
                return new Version(latestVersionInMasterChangelog.Major, minorVersionNumber, 0);
            }

            else if (options.HotFix)
            {
               var hotFixVersionNumber = CheckTheVersioningNumber(latestVersionInMasterChangelog.Build,options);
               return new Version(latestVersionInMasterChangelog.Major, latestVersionInMasterChangelog.Minor, hotFixVersionNumber);
            }
            return releaseVersion;
        }

        private List<string> GetChangeLogsFromFiles(bool isHotfix, IEnumerable<FileInfo> unreleasedChangeLogFiles, Version releaseVersion)
        {
            var releasedChangeLogEntries = new List<string>
            {
               System.Environment.NewLine,
                $"## [v{releaseVersion}] – {DateTime.Now:yyyy-MM-dd}{System.Environment.NewLine}"
            };

            // Grab all the content of each markdown file and generate a release for a specific version
            foreach (var file in unreleasedChangeLogFiles)
            {
                var content = _fileHelper.GetFileContent(file.FullName);
                var entry = JsonConvert.DeserializeObject<ChangeLogEntry>(content);

                // Only add the changelog entries which are between the latest version found in changelog.md and the version provided
                // as the command line argument.
                if (isHotfix && !entry.IsHotFix)
                {
                    continue;
                }

                var changeLogLine = entry.FullChangeLogMessage.Trim('\r', '\n');
                releasedChangeLogEntries.Add($"- {changeLogLine}");
                _fileHelper.DeleteTheFile(file.FullName);
            }
            return releasedChangeLogEntries;
        }

        public static int CheckTheVersioningNumber(int versionNumberToCompare, GenerateReleaseOptions options)
        {
            if (options.VersionPart != 0)
            {
                if (options.VersionPart >= versionNumberToCompare)
                {
                   return options.VersionPart;
                }

                else
                {
                    throw new GenerateReleaseOptionException("Versionvalue what you Set is to low");
                }
            }
            return versionNumberToCompare + 1;
        }
    }
}
