namespace Buhler.IoT.Environment.ChangeLogTool.Config
{
    using System.Diagnostics; //NOSONAR
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    [ExcludeFromCodeCoverage]
    public class AppSettings : IAppSettings
    {
        public string UnreleasedChangeLogFolderPath { get; set; }
        public string UnreleasedChangeLogFolderFullPath => Path.Combine(GetWorkingRootDir(), UnreleasedChangeLogFolderPath);
        public string MasterChangeLogFilePath { get; set; }
        public string MasterChangeLogFileFullPath => Path.Combine(GetWorkingRootDir(), MasterChangeLogFilePath);

        public string GetWorkingRootDir()
        {
            // Root directory should be there, where the .git folder is, otherwise the current application directory will used.
            var gitDirectoryPath = new DirectoryInfo(Path.GetFullPath(".git"));
#if DEBUG
            var testRepoPath = System.Environment.GetEnvironmentVariable("DEBUG_TEST_REPO_PAHT");
            if (Debugger.IsAttached && !string.IsNullOrEmpty(testRepoPath))
            {
                gitDirectoryPath = new DirectoryInfo(Path.GetFullPath(testRepoPath));
            }
#endif
            while (!gitDirectoryPath.Exists)
            {
                if (gitDirectoryPath.Parent?.Parent == null)
                {
                    return new DirectoryInfo(System.Environment.CurrentDirectory).FullName;
                }
                gitDirectoryPath = new DirectoryInfo(Path.Combine(gitDirectoryPath.Parent.Parent.FullName, ".git"));
            }
            return gitDirectoryPath.Parent!.FullName;
        }
    }
}
