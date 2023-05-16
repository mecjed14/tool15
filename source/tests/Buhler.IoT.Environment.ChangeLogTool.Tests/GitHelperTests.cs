using Buhler.IoT.Environment.ChangeLogTool.Config;
using Buhler.IoT.Environment.ChangeLogTool.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.IO;

namespace Buhler.IoT.Environment.ChangeLogTool.Tests
{
    [TestClass]
    public class GitHelperTests
    {
        [TestMethod]
        public void GitHelper_CheckOnMasterAndNoChanges_TrueCondition_Test()
        {
            //arrange
            var lockerMock = new Mock<IConsoleHelper>();
            var config = new AppSettings();
            var gitHelper = new GitHelperMock(config, lockerMock.Object);
            gitHelper.GitCommand("abc");

            //act
            var result = gitHelper.CheckOnMasterAndNoChanges();

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GitHelper_CheckOnMasterAndNoChanges_FalseCondition_Test()
        {
            //arrange
            var lockerMock = new Mock<IConsoleHelper>();
            var config = new AppSettings();
            var gitHelper = new GitHelperMock(config, lockerMock.Object);
            gitHelper.ExitCode = 1;

            //act
            var result = gitHelper.CheckOnMasterAndNoChanges();

            //assert
            Assert.IsFalse(result);
            lockerMock.Verify(o => o.LogMessage(It.Is<string>(str => str == "There are uncommited changes in the current branch, commit your changes before generate changelog!")), Times.Once);
        }

        [TestMethod]
        public void GitHelper_ListBranches_CheckOverBranchList_Test()
        {
            //arrange
            var config = new AppSettings();
            var lockerMock = new Mock<IConsoleHelper>();
            var gitHelper = new GitHelperMock(config, lockerMock.Object);
            var branches = new[]
            {
                "git branch --list"
            };

            //act
            var result = gitHelper.ListBranches();

            //assert
            CollectionAssert.AreEqual(branches, result);
        }

        [TestMethod]
        public void GitHelper_GitCommand_Test()
        {
            //arrange
            var config = new AppSettings();
            var lockerMock = new Mock<IConsoleHelper>();
            var gitHelper = new GitHelperMock(config, lockerMock.Object);          

            //act
            var result = gitHelper.GitCommand("branch");

            //assert
            Assert.AreEqual(1, gitHelper.Commands.Count);
            Assert.AreEqual("git branch", gitHelper.Commands[0]);
            Assert.AreEqual("git branch", result);
        }

        [TestMethod]
        public void GitHelper_AddFileAndCommit_Test()
        {
            //arrange
            var config = new AppSettings();
            var lockerMock = new Mock<IConsoleHelper>();
            var gitHelper = new GitHelperMock(config, lockerMock.Object);
            config.MasterChangeLogFilePath = "test.txt";
            var testFilePath = Path.Combine(config.GetWorkingRootDir(), config.MasterChangeLogFilePath);
            File.WriteAllText(testFilePath, "lalaa");

            //act
            gitHelper.AddFileAndCommit(config.MasterChangeLogFilePath);

            //assert
            var result = gitHelper.GitCommand($"status");
            Assert.IsFalse(result.Contains("nothing to commit"), "Commit was not performed.");
            result = gitHelper.GitCommand($"{config.MasterChangeLogFilePath}");
            Assert.AreEqual($"git {config.MasterChangeLogFilePath}", result.Trim());
            Assert.IsFalse(result.Contains("Adapt changelog"), "Commit was not performed.");
            Assert.IsTrue(result.Contains(config.MasterChangeLogFilePath), "File was not added to git.");

            ////cleanup
            if (File.Exists(testFilePath))
            {
                File.Delete(testFilePath);
            }
        }

        [TestMethod]
        public void GitHelper_CheckBranchesAndCreate_Test()
        {
            //arrange
            var config = new AppSettings();
            var lockerMock = new Mock<IConsoleHelper>();
            var gitHelper = new GitHelperMock(config,lockerMock.Object);
            var name = "lala-Land";

            //act
            gitHelper.CheckBranchesAndCreate(name);

            //assert
            Assert.AreEqual($"git checkout -B {name}", gitHelper.Commands[0]);
        }

        [TestMethod]
        public void GitHelper_CreateTag_Test()
        {
            //arrange
            var config = new AppSettings();
            var lockerMock = new Mock<IConsoleHelper>();
            var gitHelper = new GitHelperMock(config, lockerMock.Object);
            var name = "lala-Land";

            //act
            gitHelper.CreateTag(name);

            //assert
            Assert.AreEqual($"git tag \"{name}\"", gitHelper.Commands[0]);
        }

        [TestMethod]
        public void GitHelper_PushToRemote_Test()
        {
            //arrange
            var config = new AppSettings();
            var lockerMock = new Mock<IConsoleHelper>();
            var gitHelper = new GitHelperMock(config, lockerMock.Object);

            //act
            gitHelper.PushToRemote("1.0.0");

            //assert
            Assert.AreEqual(3, gitHelper.Commands.Count);
            Assert.AreEqual($"git push -u origin master", gitHelper.Commands[0]);
            Assert.AreEqual($"git push -u origin HEAD", gitHelper.Commands[1]);
            Assert.AreEqual($"git push origin \"1.0.0\"", gitHelper.Commands[2]);
        }

        [TestMethod]
        public void GitHelper_GetCurrentBranchName_Test()
        {
            //arrange
            var config = new AppSettings();
            var lockerMock = new Mock<IConsoleHelper>();
            var gitHelper = new GitHelperMock(config, lockerMock.Object);
            gitHelper.GitCommand("rev-parse --abbrev-ref HEAD");

            //act
            gitHelper.GetCurrentBranchName();

            //assert
            Assert.AreEqual("git rev-parse --abbrev-ref HEAD", gitHelper.Commands[0]);
        }

        [TestMethod]
        public void GitHelper_CheckForNoChanges_Test()
        {
            //arrange
            var config = new AppSettings();
            var consoleHelperMock = new Mock<IConsoleHelper>();
            var gitHelper = new GitHelperMock(config, consoleHelperMock.Object);
            gitHelper.ExitCode = 0;

            //act
            var result = gitHelper.CheckForNoChanges();

            //assert
            Assert.AreEqual(1, gitHelper.Commands.Count);
            Assert.AreEqual("git diff --exit-code", gitHelper.Commands[0]);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GitHelper_CheckForNoChanges_WithChanges_Test()
        {
            //arrange
            var config = new AppSettings();
            var consoleHelperMock = new Mock<IConsoleHelper>();
            var gitHelper = new GitHelperMock(config, consoleHelperMock.Object);
            gitHelper.ExitCode = -1;

            //act
            var result = gitHelper.CheckForNoChanges();

            //assert
            Assert.AreEqual(1, gitHelper.Commands.Count);
            Assert.AreEqual("git diff --exit-code", gitHelper.Commands[0]);
            Assert.IsFalse(result);
        }

        private class GitHelperMock : GitHelper
        {
            public List<string> Commands { get; } = new List<string>();

            public int ExitCode { get; set; }

            public GitHelperMock(AppSettings appSettings, IConsoleHelper consoleHelper)
                : base(appSettings, consoleHelper)
            {

            }

            protected override string GitCommand(string command, out int exitCode)
            {
                exitCode = ExitCode;
                var executedCommand = $"git {command}";
                Commands.Add(executedCommand);
                return executedCommand;
            }
        }
    }
}
