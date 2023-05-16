namespace Buhler.IoT.Environment.ChangeLogTool.ChangeLog
{
    using System;

    public class ChangeLogEntry
    {
        private Guid _guid;

        public ChangeLogEntry()
        {
            _guid = Guid.NewGuid();
        }

        public string Id => _guid.ToString();
        public string IssueId { get; set; }
        public string Prefix { get; set; }
        public string Text{ get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsHotFix { get; set; }
        public string CreatedBy { get; set; }
        public string FullChangeLogMessage => $"{Prefix}: {Text} ({IssueId})";
    }
}
