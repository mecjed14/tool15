using Buhler.IoT.Environment.ChangeLogTool.CommandLineOptions;


namespace Buhler.IoT.Environment.ChangeLogTool.Tools
{
    public interface IEntryCreator
    {
        int ParseAndExecuteAddMessageOptions(AddMessageOptions options);
    }
}
