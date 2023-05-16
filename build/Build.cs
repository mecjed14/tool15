using Newtonsoft.Json;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitLab;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using Serilog;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using Nuke.Common.Tools.SonarScanner;
using System.Collections.Generic;
using Nuke.Common.Tooling;
using static Nuke.Common.EnvironmentInfo;


[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main() => Execute<Build>(x => x.Default);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter]
    readonly string SonarHostUrl;

    [Parameter]
    [Secret]
    readonly string SonarToken;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion] readonly GitVersion GitVersion;

    readonly string ProjectKey = "Buhler-Changelog-Tool";

    AbsolutePath SourceDirectory => RootDirectory / "source";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath TestResultsDirectory => ArtifactsDirectory / "testresults";

    Target Info => _ => _
    .Before(Clean)
    .Executes(() =>
    {
        var gitversionJson = JsonConvert.SerializeObject(GitVersion, Formatting.Indented);
        Log.Information(gitversionJson);
        Log.Information($"Configuration {Configuration}");
    });


    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(ArtifactsDirectory);
        });
   
    Target Restore => _ => _
        .Before(SonarBegin)
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target SonarBegin => _ => _
        .Before(Compile)
        .OnlyWhenStatic(() => !SonarToken.IsNullOrEmpty() && !SonarHostUrl.IsNullOrEmpty())
        .Executes(() =>
        {
            var buildServerName = string.Empty;

            var projectId = string.Empty;
            var pipelineUrl = string.Empty;
            var commitRefName = string.Empty;
            var commitSha = string.Empty;
            var mergeRequestId = string.Empty;
            var sourceBranch = string.Empty;
            var targetBranch = string.Empty;
            var defaultBranch = "master";

            var pullrequestDecorationSystem = string.Empty;
            var pullrequestDecorationProperties = new Dictionary<string, string>();

            if (GitLab.Instance != null)
            {
                Log.Information("Running on {Host}...", nameof(GitLab));
                buildServerName = nameof(GitLab);
                projectId = GitLab.Instance.ProjectId.ToString();
                mergeRequestId = GetVariable<string>("CI_MERGE_REQUEST_IID");
                sourceBranch = GetVariable<string>("CI_MERGE_REQUEST_SOURCE_BRANCH_NAME");
                targetBranch = GetVariable<string>("CI_MERGE_REQUEST_TARGET_BRANCH_NAME");
                defaultBranch = GetVariable<string>("CI_DEFAULT_BRANCH");
                pipelineUrl = $"{GitLab.Instance.ProjectUrl}/pipelines";

                pullrequestDecorationSystem = "gitlab";
                pullrequestDecorationProperties.Add($"sonar.pullrequest.{pullrequestDecorationSystem}.instanceUrl", GetVariable<string>("CI_API_V4_URL"));
                pullrequestDecorationProperties.Add($"sonar.pullrequest.{pullrequestDecorationSystem}.projectId", GetVariable<string>("CI_PROJECT_PATH"));
                pullrequestDecorationProperties.Add($"sonar.pullrequest.{pullrequestDecorationSystem}.projectUrl", GetVariable<string>("CI_MERGE_REQUEST_PROJECT_URL"));
                pullrequestDecorationProperties.Add($"com.github.mc1arke.sonarqube.plugin.branch.pullrequest.{pullrequestDecorationSystem}.pipelineId", GetVariable<string>("CI_PIPELINE_ID"));
            }


            commitSha = GitVersion.Sha;
            commitRefName = GitVersion.BranchName;

            var analysisParameters = new
            {
                SonarProjectKey = ProjectKey,
                Version = GitVersion.NuGetVersionV2,
                ProjectId = projectId,
                RefName = commitRefName,
                MergeRequestId = mergeRequestId,
                MergeRequestSourceBranch = sourceBranch,
                MergeRequestTargetBranch = targetBranch,
                CommitId = commitSha,
                ServerName = buildServerName,
                ServerBuild = IsServerBuild,
                PipelineUrl = pipelineUrl,
            };

            Log.Information("Analysis Parameters:{@AnalysisParameters}", analysisParameters);

            var sonarBeginSettings = new SonarScannerBeginSettings()
                .SetServer(SonarHostUrl)
                .SetProjectKey(ProjectKey)
                .SetLogin(SonarToken)
                .SetVersion(GitVersion.NuGetVersionV2)
                .SetSourceEncoding("UTF-8")
                .SetFramework("netcoreapp3.0")
                .SetVSTestReports(TestResultsDirectory / "*.trx")
                .SetOpenCoverPaths(TestResultsDirectory / "**" / "coverage.opencover.xml")
                .AddDuplicationExclusions(
                    "**/AssemblyInfo.cs"
                )
                .When(IsServerBuild, c => c.SetSCMUrl(GitRepository.HttpsUrl))
                .When(IsServerBuild, c => c.SetContinuousIntegrationUrl(pipelineUrl))
                .SetProcessArgumentConfigurator(_ =>
                {
                    _.Add("/d:sonar.projectBaseDir={value}", SourceDirectory);
                    _.Add("/d:sonar.verbose=true", Verbosity == Verbosity.Verbose);
                    _.Add("/d:sonar.qualitygate.wait=true");

                    if (IsServerBuild && !mergeRequestId.IsNullOrEmpty())
                    {
                        _.Add("/d:sonar.pullrequest.key={value}", mergeRequestId);
                        _.Add("/d:sonar.pullrequest.branch={value}", sourceBranch);
                        _.Add("/d:sonar.pullrequest.base={value}", targetBranch);
                        foreach (var property in pullrequestDecorationProperties)
                        {
                            _.Add($"/d:{property.Key}={{value}}", property.Value);
                        }
                    }
                    else
                    {
                        _.Add("/d:sonar.branch.name={value}", commitRefName);

                        if (commitRefName != defaultBranch)
                        {
                            _.Add("/d:sonar.newCode.referenceBranch={value}", defaultBranch);
                        }
                    }

                    return _;
                });

            SonarScannerTasks.SonarScannerBegin(sonarBeginSettings);
        });

    Target Compile => _ => _
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .EnableNoRestore());
        });

    Target Test => _ => _
        .After(Compile)
        .Executes(() =>
        {
            var projects = Solution.GetProjects("*.Tests");
            string LogFileNameFormat(string id) => $"Tests-{id}.trx";
            DotNetTest(s => s
                .SetConfiguration(Configuration)
                .EnableNoBuild()
                .SetResultsDirectory(TestResultsDirectory)
                .SetDataCollector("XPlat Code Coverage")
                .SetRunSetting("DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format", "opencover,cobertura")
                .CombineWith(
                    projects, (cs, v) => cs
                        .SetLoggers($"trx;LogFileName={LogFileNameFormat(v.Name)}")
                        .SetProjectFile(v)));
                
        });

    Target SonarEnd => _ => _
        .After(Test)
        .OnlyWhenStatic(() => !SonarToken.IsNullOrEmpty())
        .Executes(() =>
        {
            var sonarEndSettings = new SonarScannerEndSettings()
                .SetLogin(SonarToken)
                .SetFramework("netcoreapp3.0");

            SonarScannerTasks.SonarScannerEnd(sonarEndSettings);
        });

    Target Default => _ => _
        .DependsOn(Info)
        .DependsOn(Clean)
        .DependsOn(SonarBegin)
        .DependsOn(Restore)
        .DependsOn(Compile)
        .DependsOn(Test)
        .DependsOn(SonarEnd)
    ;
}
