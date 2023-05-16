using Buhler.IoT.Environment.ChangeLogTool.ChangeLog;
using Buhler.IoT.Environment.ChangeLogTool.CommandLineOptions;
using Buhler.IoT.Environment.ChangeLogTool.Config;
using Buhler.IoT.Environment.ChangeLogTool.Exceptions;
using Buhler.IoT.Environment.ChangeLogTool.Helper;
using Buhler.IoT.Environment.ChangeLogTool.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Buhler.IoT.Environment.ChangeLogTool.Tests
{
    [TestClass]
    public class ReleaserTests
    {
        [TestMethod]
        public void Releaser_ParseAndExecuteGenerateReleaseOptions_CheckTheFolderForTheFile_Test()
        {
            //arrange
            var loggerMock = new Mock<IConsoleHelper>();
            var directorServiceMock = new Mock<IDirectoryService>();
            var filerHelperMock = new Mock<IFileHelper>();
            var gitHelperMock = new Mock<IGitHelper>();
            var releaserManagerMock = new Mock<IReleaseManager>();
            var config = new AppSettings();
            var options = new GenerateReleaseOptions();
            var creator = new Releaser(loggerMock.Object, directorServiceMock.Object, filerHelperMock.Object, releaserManagerMock.Object, config);
            config.UnreleasedChangeLogFolderPath = "anfkam";
            options.Major = true;
            directorServiceMock.Setup(o => o.EnsureDirectory(It.IsAny<string>())).Returns(new DirectoryInfo("lala-Land"));
            gitHelperMock.Setup(o => o.CheckOnMasterAndNoChanges()).Returns(true);

            //act
            var result = creator.ParseAndExecuteGenerateReleaseOptions(options);

            //assert
            Assert.AreEqual(3, result);
            loggerMock.Verify(o => o.LogMessage(It.Is<string>(str => str == $"There are no *.json files in the directory {config.UnreleasedChangeLogFolderFullPath} to process.")));
        }

        [TestMethod]
        public void Releaser_ParseAndExecuteGenerateReleaseOptions_ReleaseCheckResult_Test()
        {
            //arrange
            var loggerMock = new Mock<IConsoleHelper>();
            var directorServiceMock = new Mock<IDirectoryService>();
            var filerHelperMock = new Mock<IFileHelper>();
            var releaserMock = new Mock<IReleaseManager>();
            var config = new AppSettings();
            var options = new GenerateReleaseOptions();
            var creator = new Releaser(loggerMock.Object, directorServiceMock.Object, filerHelperMock.Object, releaserMock.Object, config);
            releaserMock.Setup(o => o.CheckTheRelease(It.IsAny<GenerateReleaseOptions>())).Returns(1);

            //act
            var result = creator.ParseAndExecuteGenerateReleaseOptions(options);

            //assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void Releaser_ParseAndExecuteGenerateReleaseOptions_GetReleaseVersion_MajorConditionTest()
        {
            //arrange
            var loggerMock = new Mock<IConsoleHelper>();
            var filerHelperMock = new Mock<IFileHelper>();
            var directorServiceMock = new Mock<IDirectoryService>();
            var gitHelperMock = new Mock<IGitHelper>();
            var releaserManagerMock = new Mock<IReleaseManager>();
            var options = new GenerateReleaseOptions();
            var config = new AppSettings();
            var changelogMock = new ChangeLogEntry
            {
                Prefix = "hubert",
                Text = "text",
                IssueId = "issueId"
            };
            var changelogMockJson = JsonConvert.SerializeObject(changelogMock);
            var creator = new Releaser(loggerMock.Object, directorServiceMock.Object, filerHelperMock.Object, releaserManagerMock.Object, config);
            directorServiceMock.Setup(o => o.EnsureDirectory(It.IsAny<string>())).Returns(new DirectoryInfo("lala-Land"));
            filerHelperMock.Setup(o => o.GetAllChangelogFiles(It.IsAny<DirectoryInfo>())).Returns(new[] { new FileInfo("non-existing.json") });
            filerHelperMock.Setup(o => o.ReadAllLineOfTheFiles(It.IsAny<string>())).Returns(new [] { "", "", "## [v1.0.0] – 2021-04-30" });
            filerHelperMock.Setup(o => o.GetFileContent(It.IsAny<string>())).Returns(changelogMockJson);
            gitHelperMock.Setup(o => o.CheckOnMasterAndNoChanges()).Returns(true);
            config.UnreleasedChangeLogFolderPath = "log";
            config.MasterChangeLogFilePath = "plot";
            options.Major = true;
            options.Push = true;

            //act
            var result = creator.ParseAndExecuteGenerateReleaseOptions(options);

            //assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void Releaser_ParseAndExecuteGenerateReleaseOptions_GetReleaseVersion_MinorConditionTest()
        {
            //arrange
            var loggerMock = new Mock<IConsoleHelper>();
            var filerHelperMock = new Mock<IFileHelper>();
            var directorServiceMock = new Mock<IDirectoryService>();
            var gitHelperMock = new Mock<IGitHelper>();
            var releaserManagerMock = new Mock<IReleaseManager>();
            var options = new GenerateReleaseOptions();
            var config = new AppSettings();
            var changelogMock = new ChangeLogEntry
            {
                Prefix = "hubert",
                Text = "text",
                IssueId = "issueId"
            };
            var changelogMockJson = JsonConvert.SerializeObject(changelogMock);
            var creator = new Releaser(loggerMock.Object, directorServiceMock.Object, filerHelperMock.Object, releaserManagerMock.Object, config);
            directorServiceMock.Setup(o => o.EnsureDirectory(It.IsAny<string>())).Returns(new DirectoryInfo("lala-Land"));
            filerHelperMock.Setup(o => o.GetAllChangelogFiles(It.IsAny<DirectoryInfo>())).Returns(new[] { new FileInfo("non-existing.json") });
            filerHelperMock.Setup(o => o.ReadAllLineOfTheFiles(It.IsAny<string>())).Returns(new [] { "", "", "## [v1.0.0] – 2021-04-30" });
            filerHelperMock.Setup(o => o.GetFileContent(It.IsAny<string>())).Returns(changelogMockJson);
            gitHelperMock.Setup(o => o.CheckOnMasterAndNoChanges()).Returns(true);
            config.UnreleasedChangeLogFolderPath = "log";
            config.MasterChangeLogFilePath = "plot";
            options.Minor = true;
            options.Push = true;

            //act
            var result = creator.ParseAndExecuteGenerateReleaseOptions(options);

            //assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void Releaser_ParseAndExecuteGenerateReleaseOptions_GetReleaseVersion_HotFixConditionTest()
        {
            //arrange
            var loggerMock = new Mock<IConsoleHelper>();
            var filerHelperMock = new Mock<IFileHelper>();
            var directorServiceMock = new Mock<IDirectoryService>();
            var gitHelperMock = new Mock<IGitHelper>();
            var releaserManagerMock = new Mock<IReleaseManager>();
            var options = new GenerateReleaseOptions();
            var config = new AppSettings();
            var changelogMock = new ChangeLogEntry
            {
                Prefix = "hubert",
                Text = "text",
                IssueId = "issueId"
            };
            var changelogMockJson = JsonConvert.SerializeObject(changelogMock);
            var creator = new Releaser(loggerMock.Object, directorServiceMock.Object, filerHelperMock.Object, releaserManagerMock.Object, config);
            directorServiceMock.Setup(o => o.EnsureDirectory(It.IsAny<string>())).Returns(new DirectoryInfo("lala-Land"));
            filerHelperMock.Setup(o => o.GetAllChangelogFiles(It.IsAny<DirectoryInfo>())).Returns(new[] { new FileInfo("non-existing.json") });
            filerHelperMock.Setup(o => o.ReadAllLineOfTheFiles(It.IsAny<string>())).Returns(new [] { "", "", "## [v1.0.0] – 2021-04-30" });
            filerHelperMock.Setup(o => o.GetFileContent(It.IsAny<string>())).Returns(changelogMockJson);
            gitHelperMock.Setup(o => o.CheckOnMasterAndNoChanges()).Returns(true);
            config.UnreleasedChangeLogFolderPath = "log";
            config.MasterChangeLogFilePath = "plot";
            options.HotFix = true;
            options.Push = true;

            //act
            var result = creator.ParseAndExecuteGenerateReleaseOptions(options);

            //assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void Releaser_ParseAndExecuteGenerateReleaseOptions_GetReleaseVersion_PushConditionTest()
        {
            //arrange
            var loggerMock = new Mock<IConsoleHelper>();
            var filerHelperMock = new Mock<IFileHelper>();
            var directorServiceMock = new Mock<IDirectoryService>();
            var gitHelperMock = new Mock<IGitHelper>();
            var releaserManagerMock = new Mock<IReleaseManager>();
            var options = new GenerateReleaseOptions();
            var config = new AppSettings();
            var changelogMock = new ChangeLogEntry
            {
                Prefix = "hubert",
                Text = "text",
                IssueId = "issueId"
            };
            var changelogMockJson = JsonConvert.SerializeObject(changelogMock);
            var creator = new Releaser(loggerMock.Object, directorServiceMock.Object, filerHelperMock.Object, releaserManagerMock.Object, config);
            directorServiceMock.Setup(o => o.EnsureDirectory(It.IsAny<string>())).Returns(new DirectoryInfo("lala-Land"));
            filerHelperMock.Setup(o => o.GetAllChangelogFiles(It.IsAny<DirectoryInfo>())).Returns(new[] { new FileInfo("non-existing.json") });
            filerHelperMock.Setup(o => o.ReadAllLineOfTheFiles(It.IsAny<string>())).Returns(new [] { "", "", "## [v1.0.0] – 2021-04-30" });
            filerHelperMock.Setup(o => o.GetFileContent(It.IsAny<string>())).Returns(changelogMockJson);
            gitHelperMock.Setup(o => o.CheckOnMasterAndNoChanges()).Returns(true);
            config.UnreleasedChangeLogFolderPath = "log";
            config.MasterChangeLogFilePath = "plot";
            options.Push = true;

            //act
            var result = creator.ParseAndExecuteGenerateReleaseOptions(options);

            //assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void Releaser_ParseAndExecuteGenerateReleaseOptions_GetReleaseVersion_VersionValueValidityTest()
        {
            //arrange
            var loggerMock = new Mock<IConsoleHelper>();
            var filerHelperMock = new Mock<IFileHelper>();
            var directorServiceMock = new Mock<IDirectoryService>();
            var gitHelperMock = new Mock<IGitHelper>();
            var releaserManagerMock = new Mock<IReleaseManager>();
            var options = new GenerateReleaseOptions();
            var config = new AppSettings();
            var changelogMock = new ChangeLogEntry
            {
                Prefix = "hubert",
                Text = "text",
                IssueId = "issueId"
            };
            var changelogMockJson = JsonConvert.SerializeObject(changelogMock);
            var creator = new Releaser(loggerMock.Object, directorServiceMock.Object, filerHelperMock.Object, releaserManagerMock.Object, config);
            directorServiceMock.Setup(o => o.EnsureDirectory(It.IsAny<string>())).Returns(new DirectoryInfo("lala-Land"));
            filerHelperMock.Setup(o => o.GetAllChangelogFiles(It.IsAny<DirectoryInfo>())).Returns(new[] { new FileInfo("non-existing.json") });
            filerHelperMock.Setup(o => o.ReadAllLineOfTheFiles(It.IsAny<string>())).Returns(new [] { "", "", "## [v1.0.0] – 2021-04-30" });
            filerHelperMock.Setup(o => o.GetFileContent(It.IsAny<string>())).Returns(changelogMockJson);
            gitHelperMock.Setup(o => o.CheckOnMasterAndNoChanges()).Returns(true);
            config.UnreleasedChangeLogFolderPath = "log";
            config.MasterChangeLogFilePath = "plot";

            //act
            var result = creator.ParseAndExecuteGenerateReleaseOptions(options);

            //assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void Releaser_ParseAndExecuteGenerateReleaseOptions_ChangeLogEntryIndexTest()
        {
            //arrange
            var loggerMock = new Mock<IConsoleHelper>();
            var filerHelperMock = new Mock<IFileHelper>();
            var directorServiceMock = new Mock<IDirectoryService>();
            var gitHelperMock = new Mock<IGitHelper>();
            var releaserManagerMock = new Mock<IReleaseManager>();
            var options = new GenerateReleaseOptions();
            var config = new AppSettings();
            var creator = new Releaser(loggerMock.Object, directorServiceMock.Object, filerHelperMock.Object, releaserManagerMock.Object, config);
            directorServiceMock.Setup(o => o.EnsureDirectory(It.IsAny<string>())).Returns(new DirectoryInfo("lala-Land"));
            filerHelperMock.Setup(o => o.GetAllChangelogFiles(It.IsAny<DirectoryInfo>())).Returns(new[] { new FileInfo("non-existing.json") });
            filerHelperMock.Setup(o => o.ReadAllLineOfTheFiles(It.IsAny<string>())).Returns(new [] { "## [v1.0.0] – 2021-04-30" });
            gitHelperMock.Setup(o => o.CheckOnMasterAndNoChanges()).Returns(true);
            config.UnreleasedChangeLogFolderPath = "log";
            config.MasterChangeLogFilePath = "plot";
            options.HotFix = true;

            //act
            var result = creator.ParseAndExecuteGenerateReleaseOptions(options);

            //assert
            Assert.AreEqual(4, result);
            loggerMock.Verify(o => o.LogMessage(It.Is<string>(str => str == $"No latest version found in file {config.MasterChangeLogFileFullPath}, therefore the changelog entries cannot be added...")));
        }

        [TestMethod]
        public void Releaser_ParseAndExecuteGenerateReleaseOptions_SetNewVersionTest()
        {
            //arrange
            var loggerMock = new Mock<IConsoleHelper>();
            var filerHelperMock = new Mock<IFileHelper>();
            var directorServiceMock = new Mock<IDirectoryService>();
            var gitHelperMock = new Mock<IGitHelper>();
            var releaserManagerMock = new Mock<IReleaseManager>();
            var options = new GenerateReleaseOptions();
            var config = new AppSettings();
            var changelogMock = new ChangeLogEntry
            {
                Prefix = "hubert",
                Text = "text",
                IssueId = "issueId"
            };
            var changelogMockJson = JsonConvert.SerializeObject(changelogMock);
            var creator = new Releaser(loggerMock.Object, directorServiceMock.Object, filerHelperMock.Object, releaserManagerMock.Object, config);
            directorServiceMock.Setup(o => o.EnsureDirectory(It.IsAny<string>())).Returns(new DirectoryInfo("lala-Land"));
            filerHelperMock.Setup(o => o.GetAllChangelogFiles(It.IsAny<DirectoryInfo>())).Returns(new[] { new FileInfo("non-existing.json") });
            filerHelperMock.Setup(o => o.ReadAllLineOfTheFiles(It.IsAny<string>())).Returns(new [] { "" });
            filerHelperMock.Setup(o => o.GetFileContent(It.IsAny<string>())).Returns(changelogMockJson);
            gitHelperMock.Setup(o => o.CheckOnMasterAndNoChanges()).Returns(true);
            config.UnreleasedChangeLogFolderPath = "log";
            config.MasterChangeLogFilePath = "plot";
            options.Major = true;

            //act
            var result = creator.ParseAndExecuteGenerateReleaseOptions(options);

            //assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void Releaser_ParseAndExecuteGenerateReleaseOptions_masterChangeLogContent_Test()
        {
            //arrange
            var loggerMock = new Mock<IConsoleHelper>();
            var filerHelperMock = new Mock<IFileHelper>();
            var directorServiceMock = new Mock<IDirectoryService>();
            var gitHelperMock = new Mock<IGitHelper>();
            var releaserManagerMock = new Mock<IReleaseManager>();
            var options = new GenerateReleaseOptions();
            var config = new AppSettings();
            var creator = new Releaser(loggerMock.Object, directorServiceMock.Object, filerHelperMock.Object, releaserManagerMock.Object, config);
            directorServiceMock.Setup(o => o.EnsureDirectory(It.IsAny<string>())).Returns(new DirectoryInfo("lala-Land"));
            filerHelperMock.Setup(o => o.GetAllChangelogFiles(It.IsAny<DirectoryInfo>())).Returns(new[] { new FileInfo("non-existing.json") });
            filerHelperMock.Setup(o => o.ReadAllLineOfTheFiles(It.IsAny<string>())).Returns(new [] { "## [v1.0.0] – 2021-04-30" });
            gitHelperMock.Setup(o => o.CheckOnMasterAndNoChanges()).Returns(true);
            config.UnreleasedChangeLogFolderPath = "log";
            config.MasterChangeLogFilePath = "plot";
            options.HotFix = true;

            //act
            creator.ParseAndExecuteGenerateReleaseOptions(options);

            //assert
            filerHelperMock.Verify(o => o.ReadAllLineOfTheFiles(config.MasterChangeLogFileFullPath), Times.Once);
        }

        [TestMethod]
        public void Releaser_ParseAndExecuteGenerateReleaseOptions_CheckContent_Test()
        {
            //arrange
            var loggerMock = new Mock<IConsoleHelper>();
            var filerHelperMock = new Mock<IFileHelper>();
            var directorServiceMock = new Mock<IDirectoryService>();
            var gitHelperMock = new Mock<IGitHelper>();
            var releaserManagerMock = new Mock<IReleaseManager>();
            var options = new GenerateReleaseOptions();
            var config = new AppSettings();
            var changelogMock = new ChangeLogEntry
            {
                Prefix = "hubert",
                Text = "text",
                IssueId = "issueId"
            };
            var changelogMockJson = JsonConvert.SerializeObject(changelogMock);
            var creator = new Releaser(loggerMock.Object, directorServiceMock.Object, filerHelperMock.Object, releaserManagerMock.Object, config);
            directorServiceMock.Setup(o => o.EnsureDirectory(It.IsAny<string>())).Returns(new DirectoryInfo("lala-Land"));
            filerHelperMock.Setup(o => o.GetAllChangelogFiles(It.IsAny<DirectoryInfo>())).Returns(new[] { new FileInfo("non-existing.json") });
            filerHelperMock.Setup(o => o.ReadAllLineOfTheFiles(It.IsAny<string>())).Returns(new [] { "" });
            filerHelperMock.Setup(o => o.GetFileContent(It.IsAny<string>())).Returns(changelogMockJson);
            gitHelperMock.Setup(o => o.CheckOnMasterAndNoChanges()).Returns(true);
            config.UnreleasedChangeLogFolderPath = "log";
            config.MasterChangeLogFilePath = "plot";
            options.Major = true;
            var file = new FileInfo("non-existing.json");

            //act
            creator.ParseAndExecuteGenerateReleaseOptions(options);

            //assert
            filerHelperMock.Verify(o => o.GetFileContent(file.FullName),Times.Once);
        }

        [TestMethod]
        public void Releaser_ParseAndExecuteGenerateReleaseOptions_DeleteTheFile_Test()
        {
            //arrange
            var loggerMock = new Mock<IConsoleHelper>();
            var filerHelperMock = new Mock<IFileHelper>();
            var directorServiceMock = new Mock<IDirectoryService>();
            var gitHelperMock = new Mock<IGitHelper>();
            var releaserManagerMock = new Mock<IReleaseManager>();
            var options = new GenerateReleaseOptions();
            var config = new AppSettings();
            var changelogMock = new ChangeLogEntry
            {
                Prefix = "hubert",
                Text = "text",
                IssueId = "issueId"
            };
            var changelogMockJson = JsonConvert.SerializeObject(changelogMock);
            var creator = new Releaser(loggerMock.Object, directorServiceMock.Object, filerHelperMock.Object, releaserManagerMock.Object, config);
            directorServiceMock.Setup(o => o.EnsureDirectory(It.IsAny<string>())).Returns(new DirectoryInfo("lala-Land"));
            filerHelperMock.Setup(o => o.GetAllChangelogFiles(It.IsAny<DirectoryInfo>())).Returns(new[] { new FileInfo("non-existing.json") });
            filerHelperMock.Setup(o => o.ReadAllLineOfTheFiles(It.IsAny<string>())).Returns(new [] { "" });
            filerHelperMock.Setup(o => o.GetFileContent(It.IsAny<string>())).Returns(changelogMockJson);
            gitHelperMock.Setup(o => o.CheckOnMasterAndNoChanges()).Returns(true);
            config.UnreleasedChangeLogFolderPath = "log";
            config.MasterChangeLogFilePath = "plot";
            options.Major = true;
            var file = new FileInfo("non-existing.json");

            //act
            creator.ParseAndExecuteGenerateReleaseOptions(options);

            //assert
            filerHelperMock.Verify(o => o.DeleteTheFile(file.FullName), Times.Once);
        }

        [TestMethod]
        public void Releaser_CheckTheMajorNumber_WhenTheVersionValueIsBiggerThenVersionNumber_Test()
        {
            var latestVersionInMasterChangeLog = new Version(16,0,0);
            var options = new GenerateReleaseOptions
            {
                VersionPart = 16
            };

            //act
            Releaser.CheckTheVersioningNumber(latestVersionInMasterChangeLog.Major,options);

            //assert
            Assert.AreEqual(options.VersionPart, latestVersionInMasterChangeLog.Major);
        }

        [TestMethod]
        public void Releaser_CheckTheMajorNumber_WhenTheVersionValueIsLowerThenVersionNumber_Test()
        {
            //arrange
            var latestVersionInMasterChangeLog = new Version(16, 0, 0);
            var options = new GenerateReleaseOptions
            {
                VersionPart = 5
            };

            //act andassert
            Assert.ThrowsException<GenerateReleaseOptionException>(() => Releaser.CheckTheVersioningNumber(latestVersionInMasterChangeLog.Major,options));
        }

        [TestMethod]
        public void Releaser_CheckTheMinorNumber_WhenTheVersionValueIsBiggerThenVersionNumber_Test()
        {
            //arrange
            var latestVersionInMasterChangeLog = new Version(16, 16, 0);
            var options = new GenerateReleaseOptions
            {
                VersionPart = 16
            };

            //act
            Releaser.CheckTheVersioningNumber(latestVersionInMasterChangeLog.Minor,options);

            //assert
            Assert.AreEqual(options.VersionPart, latestVersionInMasterChangeLog.Minor);
        }

        [TestMethod]
        public void Releaser_CheckTheMinorNumber_WhenTheVersionValueIsLowerThenVersionNumber_Test()
        {
            //arrange
            var latestVersionInMasterChangeLog = new Version(5, 16, 0);
            var options = new GenerateReleaseOptions
            {
                VersionPart = 5
            };

            //act andassert
            Assert.ThrowsException<GenerateReleaseOptionException>(() => Releaser.CheckTheVersioningNumber(latestVersionInMasterChangeLog.Minor,options));
        }

        [TestMethod]
        public void Releaser_CheckTheHotfixNumber_WhenTheVersionValueIsBiggerThenVersionNumber_Test()
        {
            //arrange
            var latestVersionInMasterChangeLog = new Version(13, 6, 16);
            var options = new GenerateReleaseOptions
            {
                VersionPart = 16
            };

            //act
            Releaser.CheckTheVersioningNumber(latestVersionInMasterChangeLog.Build,options);

            //assert
            Assert.AreEqual(options.VersionPart, latestVersionInMasterChangeLog.Build);
        }

        [TestMethod]
        public void Releaser_CheckTheHotfixNumber_WhenTheVersionValueIsLowerThenVersionNumber_Test()
        {
            //arrange
            var latestVersionInMasterChangeLog = new Version(5, 16, 18);
            var options = new GenerateReleaseOptions
            {
                VersionPart = 5
            };

            //act andassert
            Assert.ThrowsException<GenerateReleaseOptionException>(() => Releaser.CheckTheVersioningNumber(latestVersionInMasterChangeLog.Build, options));
        }
    }
}

