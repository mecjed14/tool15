namespace Buhler.IoT.Environment.ChangeLogTool.CommandLineOptions
{
    using CommandLine;

    [Verb("add", HelpText = "Add a new changelog message")]
    public class AddMessageOptions
    {
        [Option('f', "fixed", Group = "add", HelpText = "Add changelog entry 'fixed'")]
        public bool IsFixedMessage { get; set; }

        [Option('a', "added", Group = "add", HelpText = "Add changelog entry 'added'")]
        public bool IsAddedMessage { get; set; }

        [Option('c', "changed", Group = "add", HelpText = "Add changelog entry 'changed'")]
        public bool IsChangedMessage { get; set; }

        [Option('r', "removed", Group = "add", HelpText = "Add changelog entry 'removed'")]
        public bool IsRemovedMessage { get; set; }

        [Option('i', "issueId", HelpText = "Set manually a issue id. (eg PBUZIOT-12345")]
        public string IssueId { get; set; }

        [Option('t', "text", Required = false, HelpText = "The description of the change log entry.")]
        public string Text { get; set; }

        [Option('h', "isHotFix", Required = false, HelpText = "Is the entry for a Hotfix?")]
        public bool IsHotFix { get; set; }

        public bool IsValidAddMessage()
        {
            return (IsFixedMessage || IsAddedMessage || IsChangedMessage || IsRemovedMessage);
        }
    }
}
