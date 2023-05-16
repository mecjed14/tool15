using Buhler.IoT.Environment.ChangeLogTool.CommandLineOptions;
using Buhler.IoT.Environment.ChangeLogTool.Config;
using Buhler.IoT.Environment.ChangeLogTool.Helper;
using System;
using System.Diagnostics; //NOSONAR

namespace Buhler.IoT.Environment.ChangeLogTool.Tools
{
    public class ReleaseManager : IReleaseManager
    {
        private readonly IConsoleHelper _consoleHelper;
        private readonly IGitHelper _gitHelper;

        public ReleaseManager(IConsoleHelper consoleHelper, IGitHelper gitHelper)
        {
            _consoleHelper = consoleHelper;
            _gitHelper = gitHelper;
        }

        public int CheckTheRelease(GenerateReleaseOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);
            
            if (!_gitHelper.CheckOnMasterAndNoChanges())
            {
                return 5;
            }

            if (options.Major || options.Minor || options.HotFix)
            {
                if (CheckVersioningOptions(options) != 0)
                {
                    return 1;
                }

                return IsPerformReleaseConditionCheck(options);
            }

            else if (options.Push)
            {
                return IsPerformReleaseConditionCheck(options);
            }
            _consoleHelper.LogEmptyLine();
            return 0;
        }

        public int CheckVersioningOptions (GenerateReleaseOptions options)
        {
            if ((options.Major ? 1 : 0) + (options.Minor ? 1 : 0) + (options.HotFix ? 1 : 0) != 1)
            {
                _consoleHelper.LogMessage("To generate a release one of the options must be selected (-M \"major\", -m \"minor\" or -h \"hotfix\")");
                return 1;
            }
            return 0;
        }
       
        public bool IsPerformRelease(GenerateReleaseOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);
            
            var desiredRelease = string.Empty;
            if (options.HotFix)
            {
                desiredRelease = nameof(options.HotFix);
            }
            else if (options.Minor)
            {
                desiredRelease = nameof(options.Minor);
            }
            else if (options.Major)
            {
                desiredRelease = nameof(options.Major);
            }

            _consoleHelper.LogMessage($"Proceed to create a {desiredRelease} Release? Y/N:");
            return _consoleHelper.CheckTheKey(ConsoleKey.Y);
        }

        public int IsPerformReleaseConditionCheck(GenerateReleaseOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);
            
            if (IsPerformRelease(options))
            {
#if DEBUG
                if (!Debugger.IsAttached)
                {
#endif
                    return 2;
#if DEBUG
                }
#endif
            }
            return 0;
        }

        public void BranchCreator(GenerateReleaseOptions options, Version version, IAppSettings config)
        {
            ArgumentNullException.ThrowIfNull(options);
            ArgumentNullException.ThrowIfNull(config);
            
            if (options.Push && (options.Minor || options.Major || options.HotFix))
            {
                _gitHelper.AddFileAndCommit(config.MasterChangeLogFilePath);
                var fullVersionString = $"v{version}";
                var branchName = $"release/{fullVersionString}";
                _gitHelper.CheckBranchesAndCreate(branchName);
                _gitHelper.CreateTag(fullVersionString);
                _gitHelper.PushToRemote(fullVersionString);
                _consoleHelper.LogMessage($"Successfully generated file {config.MasterChangeLogFileFullPath}");
            }
        }
    }
}
