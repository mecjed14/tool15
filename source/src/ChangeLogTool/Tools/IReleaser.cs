using Buhler.IoT.Environment.ChangeLogTool.CommandLineOptions;

namespace Buhler.IoT.Environment.ChangeLogTool.Tools
{
    public interface IReleaser
    {
        int ParseAndExecuteGenerateReleaseOptions(GenerateReleaseOptions options);
    }
}
