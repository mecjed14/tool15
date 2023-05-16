namespace Buhler.IoT.Environment.ChangeLogTool.Helper
{
    using Config;
    using System;
    using System.Diagnostics;

    public class GitHelper : IGitHelper 
    {
        private const string GitCommannd = "git";
        private readonly IAppSettings _appSettings;
        private readonly IConsoleHelper _consoleHelper;

        public GitHelper(IAppSettings appSettings, IConsoleHelper consoleHelper)
        {
            _appSettings = appSettings;
            _consoleHelper = consoleHelper;
        }

        public string GetCurrentBranchName() => GitCommand("rev-parse --abbrev-ref HEAD");

        public string[] ListBranches()
        {
            var content = GitCommand("branch --list");
            return content.Split(Environment.NewLine);
        }

        public virtual string GitCommand(string command)
        {
            return GitCommand(command, out _);
        }

        protected virtual string GitCommand(string command, out int exitCode)
        {
            var procStartInfo = new ProcessStartInfo(GitCommannd, command) //NOSONAR
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WorkingDirectory = _appSettings.GetWorkingRootDir()
            };
            using var process = new Process
            {
                StartInfo = procStartInfo,
            };
            process.Start();
            var result = process.StandardOutput.ReadToEnd();
            exitCode = process.ExitCode;
            return result;
        }

        public void AddFileAndCommit(string path)
        {
            GitCommand($"add {path}");
            GitCommand($"commit -m \"Adapt changelog\"");
        }

        public string CheckBranchesAndCreate(string branchName) => GitCommand($"checkout -B {branchName}");

        public void CreateTag(string versionString)
        {
            GitCommand($"tag \"{versionString}\"");
        }

        public void PushToRemote(string versionString)
        {
            GitCommand("push -u origin master");
            GitCommand("push -u origin HEAD");
            GitCommand($"push origin \"{versionString}\"");
        }

        public bool CheckForNoChanges()
        {
            GitCommand("diff --exit-code", out var exitCode);
            return exitCode == 0;
        }

        public  bool CheckOnMasterAndNoChanges()
        {
            var hasNoChanges = CheckForNoChanges();
            if (hasNoChanges)
            {
                GitCommand("checkout master");
            }
            else
            {
                _consoleHelper.LogMessage("There are uncommited changes in the current branch, commit your changes before generate changelog!");
                return false;
            }

            return true;
        }
    }
}
