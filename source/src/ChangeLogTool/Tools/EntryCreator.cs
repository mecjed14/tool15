namespace Buhler.IoT.Environment.ChangeLogTool.Tools
{
    using Helper;
    using ChangeLog;
    using CommandLineOptions;
    using System;
    using System.Text.RegularExpressions;

    public class EntryCreator : IEntryCreator
    {
        private readonly IConsoleHelper _consoleHelper;
        private readonly IGitHelper _gitHelper;
        private readonly IFileHelper _fileHelper;

        private readonly Regex _branchRegex = new Regex(@"^feature\/(PBUZIOT-[0-9]*)\/.*$");
        private readonly Regex _issueIdRegex = new Regex(@"^PBUZIOT-[0-9]*$");

        public EntryCreator(IConsoleHelper consoleHelper, IGitHelper gitHelper, IFileHelper fileHelper)
        {
            _consoleHelper = consoleHelper;
            _gitHelper = gitHelper;
            _fileHelper = fileHelper;
        }

        public int ParseAndExecuteAddMessageOptions(AddMessageOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);

            string prefix;
            if (options.IsAddedMessage)
            {
                prefix = "Added";
            }

            else if (options.IsChangedMessage)
            {
                prefix = "Changed";
            }

            else if (options.IsFixedMessage)
            {
                prefix = "Fixed";
            }

            else if (options.IsRemovedMessage)
            {
                prefix = "Removed";
            }

            else
            {
                throw new ArgumentException("No valid option is set!");
            }

            if (string.IsNullOrEmpty(options.Text))
            {
                _consoleHelper.LogMessage("Please enter your changelog message...");
                _consoleHelper.LogMessage(prefix);
                options.Text = _consoleHelper.ReadLine();
            }

            var changeLogEntry = GetChangeLogEntry(prefix, options);
            if (changeLogEntry == null)
            {
                return -1;
            }

            _fileHelper.WriteChangeLogEntry(changeLogEntry);
            return 0;
        }

        private ChangeLogEntry GetChangeLogEntry(string prefix, AddMessageOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);
            
            var issueId = options.IssueId?.Trim();

            if (string.IsNullOrEmpty(issueId))
            {
                var currentBranchName = _gitHelper.GetCurrentBranchName() ?? string.Empty;
                var match = _branchRegex.Match(currentBranchName);

                if (match.Success)
                {
                    issueId = match.Groups[1].Value;
                }

            }

            if (string.IsNullOrEmpty(issueId) || !_issueIdRegex.IsMatch(issueId))
            {
                _consoleHelper.LogMessage("Issue id is missing or in wrong format!!!");
                return null;
            }

            return new ChangeLogEntry
            {
                Text = options.Text,
                Prefix = prefix,
                IssueId = issueId,
                CreatedAt = DateTime.UtcNow,
                IsHotFix = options.IsHotFix,
                CreatedBy = Environment.UserName
            };
        }
    }
}
