using System;

namespace Buhler.IoT.Environment.ChangeLogTool.Helper
{
    public class ConsoleHelper : IConsoleHelper
    {
        public void LogException(string message, Exception exception)
        {
            ArgumentNullException.ThrowIfNull(exception);
            
            Console.WriteLine(message);
            Console.Error.WriteLine(exception.Message);
        }

        public void LogMessage(string message)
        {
            Console.WriteLine(message);
        }

        public void LogFormattedMessage(string format, params object[] args)
        {
            Console.WriteLine(format, args);
        }

        public void LogEmptyLine()
        {
            Console.WriteLine();
        }

        public string ReadLine()
        {
            return Console.ReadLine();
        }
        public bool CheckTheKey(ConsoleKey Key)
        {
            return Console.ReadKey().Key != Key;
        }
    }
}
