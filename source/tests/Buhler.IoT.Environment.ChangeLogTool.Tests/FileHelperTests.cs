
using Buhler.IoT.Environment.ChangeLogTool.ChangeLog;
using Buhler.IoT.Environment.ChangeLogTool.CommandLineOptions;
using Buhler.IoT.Environment.ChangeLogTool.Config;
using Buhler.IoT.Environment.ChangeLogTool.Helper;
using Buhler.IoT.Environment.ChangeLogTool.Wrapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Buhler.IoT.Environment.ChangeLogTool.Tests
{
    [TestClass]
    public class FileHelperTests
    {
        [TestMethod]
        public void FileHelper_WriteObjectToJson_IsFileInfoDirectoryExist_Test()
        {
            //arrange
            var fileWrapper = new FileWrapper();
            var consoleHelperMock = new Mock<IConsoleHelper>();
            var directoryMock = new Mock<IDirectoryInfoWrapper>();
            var settings = new AppSettings();
            var fileHelper = new FileHelper(settings, consoleHelperMock.Object, fileWrapper,directoryMock.Object );
            var changeLogEntry = new ChangeLogEntry
            {
                Prefix = "hubert",
                Text = "text",
                IssueId = "issueId"
            };
            var path = Path.Combine(Path.GetTempPath(), "non-existent-dir", "temp.json");
            var directory = Path.GetDirectoryName(path);
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, recursive: true);
            }
            var fileInfo = new FileInfo(path);
            //act
            fileHelper.WriteObjectToJson(changeLogEntry, path);

            // Assert
            Assert.IsTrue(fileInfo.Directory.Exists);
        }

        [TestMethod]
        public void FileHelper_WriteObjectToJson_IsFileInfoDirectoryNotExist_Test()
        {

            //arrange
            var fileWrapperMock = new Mock<IFileWrapper>();
            var consoleHelperMock = new Mock<IConsoleHelper>();
            var directoryMock = new Mock<IDirectoryInfoWrapper>();
            var settings = new AppSettings();
            var fileHelper = new FileHelper(settings, consoleHelperMock.Object, fileWrapperMock.Object, directoryMock.Object);
            var changeLogEntry = new ChangeLogEntry
            {
                Prefix = "hubert",
                Text = "text",
                IssueId = "issueId"
            };
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var filePath = Path.Combine(tempDir, "");
            fileWrapperMock.Setup(o => o.CreateFile(It.IsAny<string>())).Returns(new StreamWriter("adf"));

            //act
            fileHelper.WriteObjectToJson(changeLogEntry, filePath);

            // Assert
            Assert.IsFalse(Directory.Exists(tempDir));
            Assert.IsFalse(File.Exists(filePath));
        }

        [TestMethod]
        public void FileHelper_GetAllChangeLogFiles_CheckHowMuchItemsInTheList_Test()
        {
            //arrange
            var consoleHelperMock = new Mock<IConsoleHelper>();
            var directoryInfoWrapperMock = new Mock<IDirectoryInfoWrapper>();
            var settings = new AppSettings();
            var fileWrapper = new FileWrapper();
            var fileHelper = new FileHelper(settings, consoleHelperMock.Object, fileWrapper,directoryInfoWrapperMock.Object );
            var unreleasedDirInfo = new DirectoryInfo("path/to/directory");
            var fileInfo1 = new FileInfo("fileone.json");
            var fileInfo2 = new FileInfo("filetwo.json");
            directoryInfoWrapperMock.Setup(d => d.GetFiles(unreleasedDirInfo,"*.json")).Returns(new[] { fileInfo1, fileInfo2 });

            //act
            var files = fileHelper.GetAllChangelogFiles(unreleasedDirInfo);

            //assert
            Assert.AreEqual(2, files.Length);
        }

        [TestMethod]
        public void FileHelper_GetAllChangeLogFiles_VerifyIsGetFilesCalled_Test()
        {
            //arrange
            var consoleHelperMock = new Mock<IConsoleHelper>();
            var directoryInfoWrapperMock = new Mock<IDirectoryInfoWrapper>();
            var settings = new AppSettings();
            var fileWrapper = new FileWrapper();
            var fileHelper = new FileHelper(settings, consoleHelperMock.Object, fileWrapper, directoryInfoWrapperMock.Object);
            var unreleasedDirInfo = new DirectoryInfo("path/to/directory");
            var fileInfo1 = new FileInfo("fileone.json");
            var fileInfo2 = new FileInfo("filetwo.json");
            directoryInfoWrapperMock.Setup(d => d.GetFiles(unreleasedDirInfo, "*.json")).Returns(new[] { fileInfo1, fileInfo2 });

            //act
            fileHelper.GetAllChangelogFiles(unreleasedDirInfo);

            //assert
            directoryInfoWrapperMock.Verify(o => o.GetFiles(unreleasedDirInfo, "*.json"),Times.Once);
        }

        [TestMethod]
        public void FileHelper_ReadAllLinesOfTheFiles_CheckIsInTheFileALineAvailable_Test()
        {
            //arrange
            var consoleHelperMock = new Mock<IConsoleHelper>();
            var directoryInfoWrapperMock = new Mock<IDirectoryInfoWrapper>();
            var fileWrapper = new Mock <IFileWrapper>();
            var settings = new AppSettings();
            var fileHelper = new FileHelper(settings, consoleHelperMock.Object, fileWrapper.Object, directoryInfoWrapperMock.Object);
            fileWrapper.Setup(o => o.ReadAllLines(It.IsAny<string>())).Returns(new[] { "lala-land" });
            directoryInfoWrapperMock.Setup(o => o.GeFiles(It.IsAny<string>())).Returns(Array.Empty<FileInfo>());

            //act
            var files = fileHelper.ReadAllLineOfTheFiles("lala-land");

            //assert
            Assert.AreEqual(1, files.Length);
        }

        [TestMethod]
        public void FileHelper_ReadAllLinesOfTheFiles_VerifyIsGetReadAllLinesCalled_Test()
        {
            //arrange
            var consoleHelperMock = new Mock<IConsoleHelper>();
            var directoryInfoWrapperMock = new Mock<IDirectoryInfoWrapper>();
            var fileWrapper = new Mock<IFileWrapper>();
            var settings = new AppSettings();
            var fileHelper = new FileHelper(settings, consoleHelperMock.Object, fileWrapper.Object, directoryInfoWrapperMock.Object);
            fileWrapper.Setup(o => o.ReadAllLines(It.IsAny<string>())).Returns(new[] { "lala-land" });
            directoryInfoWrapperMock.Setup(o => o.GeFiles(It.IsAny<string>())).Returns(Array.Empty<FileInfo>());

            //act
            fileHelper.ReadAllLineOfTheFiles("lala-land");

            //assert
            fileWrapper.Verify(o => o.ReadAllLines("lala-land"),Times.Once);
        }

        [TestMethod]
        public void FileHelper_GetFileConstant_IsNotNullCheckAndVerifyIsReadAllTextCalles_Test()
        {
            //arrange
            var consoleHelperMock = new Mock<IConsoleHelper>();
            var fileWrapper = new Mock<IFileWrapper>();
            var directoryMock = new Mock<IDirectoryInfoWrapper>();
            var settings = new AppSettings();
            var fileHelper = new FileHelper(settings, consoleHelperMock.Object, fileWrapper.Object, directoryMock.Object);
            fileWrapper.Setup(o => o.ReadAllLines(It.IsAny<string>())).Returns(new[] {"lala-land"});

            //act
            var result = fileHelper.GetFileContent("lala-land");

            //assert
            Assert.AreEqual(null!, result);
            fileWrapper.Verify(o => o.ReadAllText("lala-land"), Times.Once);
        }

        [TestMethod]
        public void FileHelper_GetFileConstant_NullCheck_Test()
        {
            //arrange
            var consoleHelperMock = new Mock<IConsoleHelper>();
            var fileWrapper = new Mock<IFileWrapper>();
            var directoryMock = new Mock<IDirectoryInfoWrapper>();
            var settings = new AppSettings();
            var fileHelper = new FileHelper(settings, consoleHelperMock.Object, fileWrapper.Object, directoryMock.Object);

            //act
            var result = fileHelper.GetFileContent(null);

            //assert
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void FileHelper_WriteÁllLinesOfThisFiles_VerifyIsWriteAllLinesCalled_Test()
        {
            //arrange
            var consoleHelperMock = new Mock<IConsoleHelper> ();
            var fileWrapper = new Mock<IFileWrapper>(); 
            var directoryMock = new Mock<IDirectoryInfoWrapper>();
            var config = new AppSettings();
            var path = config.MasterChangeLogFilePath = "dsf";
            var contentList = new List<string> {"line1","line2","line3"};
            var fileHelper = new FileHelper(config, consoleHelperMock.Object, fileWrapper.Object, directoryMock.Object);
            fileWrapper.Setup(o => o.WriteAllLines(It.IsAny<string>(), It.IsAny<List<string>>()));
            
            //act
            fileHelper.WriteAllLinesOfTheFiles(path , contentList);

            //assert
            fileWrapper.Verify(o => o.WriteAllLines(path, contentList),Times.Once);
        }

        [TestMethod]
        public void FileHelper_DeleteTheFile_VerifyIsDeletedFileCalled_Test()
        {
            //arrange
            var consoleHelperMock = new Mock<IConsoleHelper>();
            var fileWrapper = new Mock<IFileWrapper>();
            var directoryMock = new Mock<IDirectoryInfoWrapper>();
            var settings = new AppSettings();
            var fileHelper = new FileHelper(settings, consoleHelperMock.Object, fileWrapper.Object, directoryMock.Object);
            var path = "lala-land";

            //act
            fileHelper.DeleteTheFile(path);

            //assert
            fileWrapper.Verify(o => o.DeleteFile(path),Times.Once);
        }

        [TestMethod]
        public void FileHelper_WriteChangeLogEntry_VerifyTheLogFormatMessage_Test()
        {
            //arrange
            var consoleHelperMock = new Mock<IConsoleHelper>();
            var fileWrapper = new FileWrapper();
            var directoryMock = new Mock<IDirectoryInfoWrapper>();
            var settings = new AppSettings();
            var fileHelper = new FileHelper(settings, consoleHelperMock.Object, fileWrapper, directoryMock.Object);
            var changeLogEntry = new ChangeLogEntry
            {
                Prefix = "hubert",
                Text = "text",
                IssueId = "issueId"
            };
            var path = Path.Combine(Path.GetTempPath(), "non-existent-dir", "temp.json");
            settings.UnreleasedChangeLogFolderPath = path;
            var directory = Path.GetDirectoryName(path);
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, recursive: true);
            }

            //act
            fileHelper.WriteChangeLogEntry(changeLogEntry);

            //assert
            consoleHelperMock.Verify(o => o.LogFormattedMessage(It.Is<string>(str => str == "A changelog json file with the following message will be created: {0}"), changeLogEntry.FullChangeLogMessage),Times.Once);
        }

        [TestMethod]
        public void FileHelper_WriteChangeLogEntry_CheckTheException_Test()
        {
            //arrange
            var consoleHelperMock = new Mock<IConsoleHelper>();
            var fileWrapper = new FileWrapper();
            var directoryMock = new Mock<IDirectoryInfoWrapper>();
            var settings = new AppSettings();
            var fileHelper = new FileHelper(settings, consoleHelperMock.Object, fileWrapper, directoryMock.Object);
            var path = Path.Combine(Path.GetTempPath(), "non-existent-dir", "temp.json");
            settings.UnreleasedChangeLogFolderPath = path;
            var directory = Path.GetDirectoryName(path);
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, recursive: true);
            }
            new FileInfo(path);

            //act

            //act am
            Assert.ThrowsException<ArgumentNullException>(() => fileHelper.WriteChangeLogEntry(null));
        }
    }
}



 

