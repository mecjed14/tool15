
using Buhler.IoT.Environment.ChangeLogTool.CommandLineOptions;
using Buhler.IoT.Environment.ChangeLogTool.Config;
using Buhler.IoT.Environment.ChangeLogTool.Helper;
using Buhler.IoT.Environment.ChangeLogTool.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace Buhler.IoT.Environment.ChangeLogTool.Tests
{
    [TestClass]
    public class ReleaserManagerTests
    {

        [TestMethod]
        public void ReleaserManager_CheckTheRelease_CheckForSetVersionNumber_Test()
        {
            //arrange
            var loggerMock = new Mock<IConsoleHelper>();
            var gitHelperMock = new Mock<IGitHelper>();
            var options = new GenerateReleaseOptions();
            var releaseManager = new ReleaseManager(loggerMock.Object, gitHelperMock.Object);
            options.Major = true;
            options.Minor = true;
            options.HotFix = true;
            gitHelperMock.Setup(o => o.CheckOnMasterAndNoChanges()).Returns(true);

            //act
            var result = releaseManager.CheckTheRelease(options);

            //assert
            Assert.AreEqual(1, result);
            loggerMock.Verify(o => o.LogMessage(It.Is<string>(str => str == "To generate a release one of the options must be selected (-M \"major\", -m \"minor\" or -h \"hotfix\")")), Times.Once);
        }

        [TestMethod]
        public void ReleaserManager_CheckRelease_PushIsFalseAndVersionValueValidityIsFalse_Test()
        {
            //arrange
            var loggerMock = new Mock<IConsoleHelper>();
            var gitHelperMock = new Mock<IGitHelper>();
            var options = new GenerateReleaseOptions();
            var releaserManager = new ReleaseManager(loggerMock.Object, gitHelperMock.Object);
            gitHelperMock.Setup(o => o.CheckOnMasterAndNoChanges()).Returns(true);

            //act
            var result = releaserManager.CheckTheRelease(options);

            //assert
            Assert.AreEqual(0, result);
            loggerMock.Verify(o => o.LogEmptyLine(), Times.Once);
        }

        [TestMethod]
        public void ReleaserManager_IsPerformReleaseConditionCheck_IsTheConditionTrue_Test()
        {
            //arrange
            var loggerMock = new Mock<IConsoleHelper>();
            var gitHelperMock = new Mock<IGitHelper>();
            var options = new GenerateReleaseOptions();
            var releaseManager = new ReleaseManager(loggerMock.Object, gitHelperMock.Object);
            options.Major = true;
            loggerMock.Setup(o => o.CheckTheKey(ConsoleKey.Y)).Returns(true);
            gitHelperMock.Setup(o => o.CheckOnMasterAndNoChanges()).Returns(true);

            //act
            var result = releaseManager.IsPerformReleaseConditionCheck(options);

            //assert
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void ReleaserManager_IsPerformReleaseConditionCheck_IsNotTheConditionTrue_Test()
        {
            //arrange
            var loggerMock = new Mock<IConsoleHelper>();
            var gitHelperMock = new Mock<IGitHelper>();
            var options = new GenerateReleaseOptions();
            var releaseManager = new ReleaseManager(loggerMock.Object, gitHelperMock.Object);
            options.Major = true;
            loggerMock.Setup(o => o.CheckTheKey(ConsoleKey.N)).Returns(true);
            gitHelperMock.Setup(o => o.CheckOnMasterAndNoChanges()).Returns(true);

            //act
            var result = releaseManager.IsPerformReleaseConditionCheck(options);

            //assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void ReleaserManager_ParseAndExecuteGenerateReleaseOptions_AskIfPerformReleaseTestTheMessage_Test()
        {
            //arrange
            var loggerMock = new Mock<IConsoleHelper>();
            var gitHelperMock = new Mock<IGitHelper>();
            var options = new GenerateReleaseOptions();
            var releaseManager = new ReleaseManager(loggerMock.Object, gitHelperMock.Object);
            options.Major = true;
            options.Push = true;
            loggerMock.Setup(o => o.CheckTheKey(ConsoleKey.Y)).Returns(true);
            gitHelperMock.Setup(o => o.CheckOnMasterAndNoChanges()).Returns(true);

            //act
            var result = releaseManager.IsPerformReleaseConditionCheck(options);

            //assert
            Assert.AreEqual(2, result);
            loggerMock.Verify(o => o.LogMessage(It.Is<string>(str => str == $"Proceed to create a {nameof(options.Major)} Release? Y/N:")));
        }

        [TestMethod]
        public void ReleaserManager_ParseAndExecuteGenerateReleaseOptions_CheckOnMasterAndNoChanges_Test()
        {
            //arrange
            var loggerMock = new Mock<IConsoleHelper>();
            var gitHelperMock = new Mock<IGitHelper>();
            var releaserManager = new ReleaseManager(loggerMock.Object, gitHelperMock.Object);
            var options = new GenerateReleaseOptions();
            gitHelperMock.Setup(o => o.CheckOnMasterAndNoChanges()).Returns(false);

            //act
            var result = releaserManager.CheckTheRelease(options);

            //assert
            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public void ReleaserManager_ParseAndExecuteGenerateReleaseOptions_CheckForArgumentNullException_Test()
        {
            //arrange
            var loggerMock = new Mock<IConsoleHelper>();
            var gitHelperMock = new Mock<IGitHelper>();
            var releaserManager = new ReleaseManager(loggerMock.Object, gitHelperMock.Object);
            var options = new GenerateReleaseOptions();
            options = null;

            //act and assert
            Assert.ThrowsException<ArgumentNullException>(() => releaserManager.CheckTheRelease(options));
        }



        [TestMethod]
        public void ReleaserManager_CheckTheRelease_CheckForDesiredRelease_Minor_Condition_Test()
        {
            //arrange
            var loggerMock = new Mock<IConsoleHelper>();
            var gitHelperMock = new Mock<IGitHelper>();
            var options = new GenerateReleaseOptions();
            var releaseManager = new ReleaseManager(loggerMock.Object, gitHelperMock.Object);
            options.Push = true;
            options.Minor = true;
            loggerMock.Setup(o => o.CheckTheKey(ConsoleKey.Y)).Returns(true);
            gitHelperMock.Setup(o => o.CheckOnMasterAndNoChanges()).Returns(true);

            //act
            var result = releaseManager.IsPerformReleaseConditionCheck(options);

            //assert
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void ReleaserManager_CheckTheRelease_CheckForDesiredRelease_Hotfix_Condition_Test()
        {
            //arrange
            var loggerMock = new Mock<IConsoleHelper>();
            var gitHelperMock = new Mock<IGitHelper>();
            var options = new GenerateReleaseOptions();
            var releaseManager = new ReleaseManager(loggerMock.Object, gitHelperMock.Object);
            options.Push = true;
            options.HotFix = true;
            loggerMock.Setup(o => o.CheckTheKey(ConsoleKey.Y)).Returns(true);
            gitHelperMock.Setup(o => o.CheckOnMasterAndNoChanges()).Returns(true);

            //act
            var result = releaseManager.CheckTheRelease(options);

            //assert
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void ReleaserManager_CheckTheRelease_CheckForDesiredRelease_Test()
        {
            //arrange
            var loggerMock = new Mock<IConsoleHelper>();
            var gitHelperMock = new Mock<IGitHelper>();
            var options = new GenerateReleaseOptions();
            var releaseManager = new ReleaseManager(loggerMock.Object, gitHelperMock.Object);
            options.Minor = true;
            loggerMock.Setup(o => o.CheckTheKey(ConsoleKey.Y)).Returns(false);
            gitHelperMock.Setup(o => o.CheckOnMasterAndNoChanges()).Returns(true);

            //act
            var result = releaseManager.CheckTheRelease(options);

            //assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void ReleaserManager_BranchCreator_Push_AddFileAndCommit_Test()
        {
            //arrange
            var loggerMock = new Mock<IConsoleHelper>();
            var gitHelperMock = new Mock<IGitHelper>();
            var version = new Version();
            var options = new GenerateReleaseOptions();
            var config = new AppSettings();
            var creator = new ReleaseManager(loggerMock.Object, gitHelperMock.Object);
            config.UnreleasedChangeLogFolderPath = "log";
            config.MasterChangeLogFilePath = "plot";
            options.Minor = true;
            options.Push = true;

            //act
            creator.BranchCreator(options, version, config);
            //assert
            gitHelperMock.Verify(o => o.AddFileAndCommit(It.Is<string>(str => str == config.MasterChangeLogFilePath)), Times.Once);
        }

        [TestMethod]
        public void ReleaserManager_BranchCreator_PushAndVersion_CheckBranchesAndCreate_Test()
        {
            //arrange
            var loggerMock = new Mock<IConsoleHelper>();
            var gitHelperMock = new Mock<IGitHelper>();
            var version = new Version(1,0,0);
            var options = new GenerateReleaseOptions();
            var config = new AppSettings();
            var creator = new ReleaseManager(loggerMock.Object, gitHelperMock.Object);
            config.UnreleasedChangeLogFolderPath = "log";
            config.MasterChangeLogFilePath = "plot";
            options.HotFix = true;
            options.Push = true;

            //act
            creator.BranchCreator(options,version, config);

            //assert
            gitHelperMock.Verify(o => o.CheckBranchesAndCreate(It.Is<string>(str => str == "release/v1.0.0")), Times.Once);
        }

        [TestMethod]
        public void ReleaserManager_BranchCreator_PushAndVersion_CreateTag_Test()
        {
            //arrange
            var loggerMock = new Mock<IConsoleHelper>();
            var gitHelperMock = new Mock<IGitHelper>();
            var version = new Version(1, 0, 0);
            var options = new GenerateReleaseOptions();
            var config = new AppSettings();
            var creator = new ReleaseManager(loggerMock.Object, gitHelperMock.Object);
            config.UnreleasedChangeLogFolderPath = "log";
            config.MasterChangeLogFilePath = "plot";
            options.Major = true;
            options.Push = true;

            //act
            creator.BranchCreator(options, version, config);

            //assert
            gitHelperMock.Verify(o => o.CreateTag(It.Is<string>(str => str == "v1.0.0")), Times.Once);
        }

        [TestMethod]
        public void ReleaserManager_BranchCreator_PushAndVersion_PushToRemote_Test()
        {
            //arrange
            var loggerMock = new Mock<IConsoleHelper>();
            var gitHelperMock = new Mock<IGitHelper>();
            var version = new Version(1, 0, 0);
            var options = new GenerateReleaseOptions();
            var config = new AppSettings();
            var creator = new ReleaseManager(loggerMock.Object, gitHelperMock.Object);
            config.UnreleasedChangeLogFolderPath = "log";
            config.MasterChangeLogFilePath = "plot";
            options.Major = true;
            options.Push = true;

            //act
            creator.BranchCreator(options, version, config);

            //assert
            gitHelperMock.Verify(o => o.PushToRemote(It.Is<string>(str => str == "v1.0.0")), Times.Once);
        }

        [TestMethod]
        public void ReleaserManager_BranchCreator_CheckIsTheMessageTrue_Test()
        {
            //arrange
            var loggerMock = new Mock<IConsoleHelper>();
            var gitHelperMock = new Mock<IGitHelper>();
            var version = new Version(1, 0, 0);
            var options = new GenerateReleaseOptions();
            var config = new AppSettings();
            var creator = new ReleaseManager(loggerMock.Object, gitHelperMock.Object);
            config.UnreleasedChangeLogFolderPath = "log";
            config.MasterChangeLogFilePath = "plot";
            options.Major = true;
            options.Push = true;

            //act
            creator.BranchCreator(options, version, config);

            //assert
            loggerMock.Verify(o => o.LogMessage(It.Is<string>(str => str == $"Successfully generated file {config.MasterChangeLogFileFullPath}")),Times.Once);
        }

        [TestMethod]
        public void ReleaserManager_CheckTheRelease_IsPushTrueCondition_Test()
        {
            //arrange
            var loggerMock = new Mock<IConsoleHelper>();
            var gitHelperMock = new Mock<IGitHelper>();
            var releaseManager = new ReleaseManager(loggerMock.Object, gitHelperMock.Object);
            var options = new GenerateReleaseOptions();
            gitHelperMock.Setup(o => o.CheckOnMasterAndNoChanges()).Returns(true);
            loggerMock.Setup(o => o.CheckTheKey(ConsoleKey.Y)).Returns(true);
            options.Major = true;
            options.Push = true;

            //act
            var result = releaseManager.CheckTheRelease(options);

            //assert
            Assert.AreEqual(2,result);
            loggerMock.Verify(o => o.LogMessage(It.Is<string>(str => str == $"Proceed to create a Major Release? Y/N:")));
        }

        [TestMethod]
        public void ReleaserManager_CheckVersioningOptions_CheckTheMajorCondition()
        {
            //arrange
            var loggerMock = new Mock<IConsoleHelper>();
            var gitHelperMock = new Mock<IGitHelper>();
            var releaseManager = new ReleaseManager(loggerMock.Object, gitHelperMock.Object);
            var options = new GenerateReleaseOptions();

            //act
            var result = releaseManager.CheckVersioningOptions(options);

            //assert
            Assert.AreEqual(1,result);
            loggerMock.Verify(o => o.LogMessage(It.Is<string>(str => str == "To generate a release one of the options must be selected (-M \"major\", -m \"minor\" or -h \"hotfix\")")), Times.Once);
        }
    }
}
