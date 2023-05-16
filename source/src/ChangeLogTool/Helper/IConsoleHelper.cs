using System;

namespace Buhler.IoT.Environment.ChangeLogTool.Helper
{
    public interface IConsoleHelper
    {
        void LogException(string message, Exception exception);
        void LogMessage(string message);
        void LogEmptyLine();
        void LogFormattedMessage(string format, params object[] args);
        string ReadLine();
        public bool CheckTheKey(ConsoleKey Key);
    }
}
