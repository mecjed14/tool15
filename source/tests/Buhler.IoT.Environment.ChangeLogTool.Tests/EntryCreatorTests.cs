using Buhler.IoT.Environment.ChangeLogTool.ChangeLog;
using Buhler.IoT.Environment.ChangeLogTool.CommandLineOptions;
using Buhler.IoT.Environment.ChangeLogTool.Config;
using Buhler.IoT.Environment.ChangeLogTool.Helper;
using Buhler.IoT.Environment.ChangeLogTool.Tools;
using Buhler.IoT.Environment.ChangeLogTool.Wrapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.IO;

namespace Buhler.IoT.Environment.ChangeLogTool.Tests
{
    [TestClass]
    public class EntryCreatorTests
    {

        [TestMethod]
        public void EntryCreatorTests_ParseAndExecuteAddMessageOptions_OptionsIsNull()
        {
            // arrange
            var loggerMok = new Mock<IConsoleHelper>();
            var creator = new EntryCreator(loggerMok.Object, null, null);

            // act
            var ex = Assert.ThrowsException<ArgumentNullException>(() => creator.ParseAndExecuteAddMessageOptions(null));

            // assert
            Assert.AreEqual("Value cannot be null. (Parameter 'options')", ex.Message);
        }

        [TestMethod]
        public void EntryCreatorTests_ParseAndExecuteAddMessageOptions_ArgumentException()
        {
            // arrange
            var loggerMok = new Mock<IConsoleHelper>();
            var creator = new EntryCreator(loggerMok.Object, null, null);
            var options = new AddMessageOptions();

            // act
            var ex = Assert.ThrowsException<ArgumentException>(() => creator.ParseAndExecuteAddMessageOptions(options));

            // assert
            Assert.AreEqual("No valid option is set!", ex.Message);
        }

        [TestMethod]
        public void EntryCreatorTests_ParseAndExecuteAddMessageOptions_TextNotSet()
        {
            // arrange

            var consoleHelperMock = new Mock<IConsoleHelper>();
            var gitHelperMock = new Mock<IGitHelper>();
            var creator = new EntryCreator(consoleHelperMock.Object, gitHelperMock.Object, null);
            var options = new AddMessageOptions();
            options.IsAddedMessage = true;
            gitHelperMock.Setup(o => o.GetCurrentBranchName()).Returns(string.Empty);
            consoleHelperMock.Setup(o => o.ReadLine()).Returns("my test console input test");

            // act
            creator.ParseAndExecuteAddMessageOptions(options);

            // assert
            Assert.AreEqual("my test console input test", options.Text);
            consoleHelperMock.Verify(o =>
                    o.LogMessage(
                        It.Is<string>(arg => arg == "Please enter your changelog message...")
                        )
                );
            consoleHelperMock.Verify(o =>
                   o.LogMessage(
                       It.Is<string>(arg => arg == "Added")
                       )
               );
            consoleHelperMock.Verify(o => o.LogMessage(It.IsAny<string>()));
            consoleHelperMock.Verify(o => o.ReadLine(), Times.Once);
        }

        [TestMethod]
        public void EntryCreatorTests_ParseAndExecuteAddMessageOptions_ChangeLogEntryNull_NoIssueId()
        {
            // arrange
            var consoleHelperMock = new Mock<IConsoleHelper>();
            var gitHelperMock = new Mock<IGitHelper>();
            var creator = new EntryCreator(consoleHelperMock.Object, gitHelperMock.Object, null);
            var options = new AddMessageOptions();
            options.IsAddedMessage = true;

            // act
            var result = creator.ParseAndExecuteAddMessageOptions(options);

            // assert
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void EntryCreatorTests_ParseAndExecuteAddMessageOptions_ChangeLogEntryNull()
        {
            // arrange
            var consoleHelperMock = new Mock<IConsoleHelper>();
            var gitHelperMock = new Mock<IGitHelper>();
            var creator = new EntryCreator(consoleHelperMock.Object, gitHelperMock.Object, null);
            var options = new AddMessageOptions();
            options.IsAddedMessage = true;
            gitHelperMock.Setup(o => o.GetCurrentBranchName()).Returns(string.Empty);

            // act
            var result = creator.ParseAndExecuteAddMessageOptions(options);

            // assert
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void EntryCreatorTests_ParseAndExecuteAddMessageOptions_GetChangeLogEntry()
        {
            // arrange
            var consoleHelperMock = new Mock<IConsoleHelper>();
            var gitHelperMock = new Mock<IGitHelper>();
            var fileHelperMock = new Mock<IFileHelper>();
            var creator = new EntryCreator(consoleHelperMock.Object, gitHelperMock.Object, fileHelperMock.Object);
            var options = new AddMessageOptions();
            var config = new AppSettings();
            options.IsAddedMessage = true;
            gitHelperMock.Setup(o => o.GetCurrentBranchName()).Returns(string.Empty);
            options.IssueId = "PBUZIOT-0";
            config.UnreleasedChangeLogFolderPath = "sdfsdf";

            // act
            var result = creator.ParseAndExecuteAddMessageOptions(options);

            // assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void EntryCreatorTests_ParseAndExecuteAddMessageOptions_GetChangeLogEntryValidateObject()
        {
            // arrange
            var consoleHelperMock = new Mock<IConsoleHelper>();
            var gitHelperMock = new Mock<IGitHelper>();
            var fileHelperMock = new Mock<IFileHelper>();
            var creator = new EntryCreator(consoleHelperMock.Object, gitHelperMock.Object, fileHelperMock.Object);
            var options = new AddMessageOptions
            {
                IsAddedMessage = true,
                IssueId = "PBUZIOT-0",
                Text = "My cool text!",
                IsHotFix = true,
            };
            gitHelperMock.Setup(o => o.GetCurrentBranchName()).Returns(string.Empty);

            // act
            var result = creator.ParseAndExecuteAddMessageOptions(options);

            // assert
            Assert.AreEqual(0, result);
            fileHelperMock.Verify(m =>
                  m.WriteChangeLogEntry(
                      It.Is<ChangeLogEntry>(arg =>
                              arg.Text == "My cool text!" &&
                              arg.Prefix == "Added" &&
                              arg.IssueId == "PBUZIOT-0" &&
                              arg.CreatedBy == System.Environment.UserName
                              )));
        }

        [TestMethod]
        public void EntryCreatorTests_ParseAndExecuteAddMessageOptions_GetChangeLogEntryValidateObject_ChangedPrefix_Changed()
        {
            // arrange
            var consoleHelperMock = new Mock<IConsoleHelper>();
            var gitHelperMock = new Mock<IGitHelper>();
            var fileHelperMock = new Mock<IFileHelper>();
            var creator = new EntryCreator(consoleHelperMock.Object, gitHelperMock.Object, fileHelperMock.Object);
            var options = new AddMessageOptions
            {
                IsChangedMessage = true,
                IssueId = "PBUZIOT-0",
                Text = "My cool text!",
                IsHotFix = true,
            };
            gitHelperMock.Setup(o => o.GetCurrentBranchName()).Returns(string.Empty);

            // act
            var result = creator.ParseAndExecuteAddMessageOptions(options);

            // assert
            Assert.AreEqual(0, result);
            fileHelperMock.Verify(m =>
                  m.WriteChangeLogEntry(
                      It.Is<ChangeLogEntry>(arg =>
                              arg.Text == "My cool text!" &&
                              arg.Prefix == "Changed" &&
                              arg.IssueId == "PBUZIOT-0" &&
                              arg.CreatedBy == System.Environment.UserName
                              )));
        }

        [TestMethod]
        public void EntryCreatorTests_ParseAndExecuteAddMessageOptions_GetChangeLogEntryValidateObject_ChangedPrefix_Fixed()
        {
            // arrange
            var consoleHelperMock = new Mock<IConsoleHelper>();
            var gitHelperMock = new Mock<IGitHelper>();
            var filerHelperMock = new Mock<IFileHelper>();
            var creator = new EntryCreator(consoleHelperMock.Object, gitHelperMock.Object, filerHelperMock.Object);
            var options = new AddMessageOptions
            {
                IsFixedMessage = true,
                IssueId = "PBUZIOT-0",
                Text = "My cool text!",
                IsHotFix = true,
            };
            gitHelperMock.Setup(o => o.GetCurrentBranchName()).Returns(string.Empty);

            // act
            var result = creator.ParseAndExecuteAddMessageOptions(options);

            // assert
            Assert.AreEqual(0, result);
            filerHelperMock.Verify(m =>
                m.WriteChangeLogEntry(
                    It.Is<ChangeLogEntry>(arg =>
                            arg.Text == "My cool text!" &&
                            arg.Prefix == "Fixed" &&
                            arg.IssueId == "PBUZIOT-0" &&
                            arg.CreatedBy == System.Environment.UserName
                            )));
        }

        [TestMethod]
        public void EntryCreatorTests_ParseAndExecuteAddMessageOptions_GetChangeLogEntryValidateObject_ChangedPrefix_Removed()
        {
            // arrange
            var consoleHelperMock = new Mock<IConsoleHelper>();
            var gitHelperMock = new Mock<IGitHelper>();
            var fileHelperMock = new Mock<IFileHelper>();
            var creator = new EntryCreator(consoleHelperMock.Object, gitHelperMock.Object, fileHelperMock.Object);
            var options = new AddMessageOptions
            {
                IsRemovedMessage = true,
                IssueId = "PBUZIOT-0",
                Text = "My cool text!",
                IsHotFix = true,
            };
            gitHelperMock.Setup(o => o.GetCurrentBranchName()).Returns(string.Empty);

            // act
            var result = creator.ParseAndExecuteAddMessageOptions(options);

            // assert
            Assert.AreEqual(0, result);
            fileHelperMock.Verify(m =>
                m.WriteChangeLogEntry(
                    It.Is<ChangeLogEntry>(arg =>
                            arg.Text == "My cool text!" &&
                            arg.Prefix == "Removed" &&
                            arg.IssueId == "PBUZIOT-0" &&
                            arg.CreatedBy == System.Environment.UserName
                            )));
        }

        [TestMethod]
        public void EntryCreatorTests_ParseAndExecuteAddMessageOptions_GetChangeLogEntry_issuey_id()
        {
            // arrange
            var consoleHelperMock = new Mock<IConsoleHelper>();
            var gitHelperMock = new Mock<IGitHelper>();
            var fileHelperMock = new Mock<IFileHelper>();
            var creator = new EntryCreator(consoleHelperMock.Object, gitHelperMock.Object, fileHelperMock.Object);
            var options = new AddMessageOptions();
            var config = new AppSettings();
            options.IsAddedMessage = true;
            gitHelperMock.Setup(o => o.GetCurrentBranchName()).Returns(string.Empty);
            options.IssueId = "PBUZIdsfdOT-0";
            config.UnreleasedChangeLogFolderPath = "sdfsdfsd";

            // act
            var result = creator.ParseAndExecuteAddMessageOptions(options);

            // assert
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void EntryCreatorTests_ParseAndExecuteAddMessageOptions_WriteChangeLogEntry_Test()
        {
            // arrange
           var consoleHelperMock = new Mock<IConsoleHelper>();
           var gitHelperMock = new Mock<IGitHelper>();
           var fileHelperMock = new Mock<IFileHelper>();
           var creator = new EntryCreator(consoleHelperMock.Object, gitHelperMock.Object, fileHelperMock.Object);
           var entryCreator = new ChangeLogEntry
           {
                Prefix = "asfsdf",
                CreatedBy = "aleksander",
                IssueId = "PBUZIOT-0",
                Text = "My cool text!",
                IsHotFix = true,
           };
           var options = new AddMessageOptions
           {
                  IsChangedMessage = true,
                  IssueId = "PBUZIOT-0",
                  Text = "My cool text!",
                  IsHotFix = true,
           };
           gitHelperMock.Setup(o => o.GetCurrentBranchName()).Returns(string.Empty);
           fileHelperMock.Setup(o => o.WriteChangeLogEntry(entryCreator));

           // act
           var result = creator.ParseAndExecuteAddMessageOptions(options);

           // assert
           Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void EntryCreatorTests_ParseAndExecuteAddMessageOptions_GetChangeLogentry_NUllorEmptyTest()
        {
            // arrange
            var consoleHelperMock = new Mock<IConsoleHelper>();
            var gitHelperMock = new Mock<IGitHelper>();
            var fileHelperMock = new Mock<IFileHelper>();
            var creator = new EntryCreator(consoleHelperMock.Object, gitHelperMock.Object, fileHelperMock.Object);
            
            var options = new AddMessageOptions
            {
                IsChangedMessage = true,
                Text = "My cool text!",
                IsHotFix = true,
            };
            gitHelperMock.Setup(o => o.GetCurrentBranchName()).Returns(string.Empty);

            // act
            creator.ParseAndExecuteAddMessageOptions(options);

            // assert
            consoleHelperMock.Verify(o => o.LogMessage(It.Is<string>(str => str == "Issue id is missing or in wrong format!!!")),Times.Once);
        }

        [TestMethod]
        public void EntryCreatorTests_ParseAndExecuteAddMessageOptions_GetChangeLogentry_MatchSuccessTest()
        {
            // arrange
            var consoleHelperMock = new Mock<IConsoleHelper>();
            var gitHelperMock = new Mock<IGitHelper>();
            var fileHelperMock = new Mock<IFileHelper>();
            var creator = new EntryCreator(consoleHelperMock.Object, gitHelperMock.Object, fileHelperMock.Object);
            var options = new AddMessageOptions
            {
                IsChangedMessage = true,
                Text = "My cool text!",
                IsHotFix = true,
            };

            gitHelperMock.Setup(o => o.GetCurrentBranchName()).Returns("feature/PBUZIOT-0/some-branch");

            // act
            var result = creator.ParseAndExecuteAddMessageOptions(options);

            // assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void EntryCreator_FileHelper_WriteChangeLogEntry_VerifyTest()
        {
            //arrange
            var consoleHelperMock = new Mock<IConsoleHelper>();
            var filewrapper = new Mock<IFileWrapper>();
            var directoryMock = new Mock<IDirectoryInfoWrapper>();
            var settings = new AppSettings();
            var stream = new MemoryStream();
            var writerMokc = new Mock<StreamWriter>(stream);
            var fileHelper = new FileHelper(settings,consoleHelperMock.Object,filewrapper.Object,directoryMock.Object);
            var changelogEntry = new ChangeLogEntry
            {
                Prefix = "bla",
                IssueId = "PBUZIOT-0",
                Text = "My cool text!",
                
            };
            settings.UnreleasedChangeLogFolderPath = "";
            filewrapper.Setup(o => o.CreateFile(It.IsAny<string>())).Returns(writerMokc.Object);

            //act

            fileHelper.WriteChangeLogEntry(changelogEntry);

            //assert
            consoleHelperMock.Verify(o => o.LogFormattedMessage(It.Is<string>(str => str == "A changelog json file with the following message will be created: {0}"), changelogEntry.FullChangeLogMessage),Times.Once);
        }
    }
} 
