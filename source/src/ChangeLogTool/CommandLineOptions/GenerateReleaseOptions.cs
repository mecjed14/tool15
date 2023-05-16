namespace Buhler.IoT.Environment.ChangeLogTool.CommandLineOptions
{
    using CommandLine;

    [Verb("g", HelpText = "Generate a new changelog for a specific release")]
    public class GenerateReleaseOptions
    {
        [Option('M', "major", Group = "release", HelpText = "Generate Major Release (this will increment the major version part v+1.0.0)")]
        public bool Major { get; set; }

        [Option('m', "minor", Group = "release", HelpText = "Generate Minor Release (this will increment the minor version part vx.+1.0)")]
        public bool Minor { get; set; }

        [Option('h', "hotfix", Group = "release", HelpText = "Generate HotFix Release (this will increment the patch version part vx.x.+1)")]
        public bool HotFix { get; set; }
        
        [Option('p', "push", HelpText = "Additionally comit changelog.md to master, create a release branch, a tag and push to remote")]
        public bool Push { get; set; }

        [Option('v', "versionPart", HelpText = "Set manual value of the selected version part")]
        public int VersionPart { get; set; }
    }
}
