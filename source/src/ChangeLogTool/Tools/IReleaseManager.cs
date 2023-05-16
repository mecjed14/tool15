using Buhler.IoT.Environment.ChangeLogTool.CommandLineOptions;
using Buhler.IoT.Environment.ChangeLogTool.Config;
using System;

namespace Buhler.IoT.Environment.ChangeLogTool.Tools
{
    public interface IReleaseManager
    {
        bool IsPerformRelease(GenerateReleaseOptions options);
        int CheckTheRelease(GenerateReleaseOptions options);
        void BranchCreator(GenerateReleaseOptions options, Version version, IAppSettings config);
    }
}
