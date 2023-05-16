using Buhler.IoT.Environment.ChangeLogTool.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Buhler.IoT.Environment.ChangeLogTool.Tools;
using Buhler.IoT.Environment.ChangeLogTool.Wrapper;
using Buhler.IoT.Environment.ChangeLogTool.Helper;

namespace Buhler.IoT.Environment.ChangeLogTool.Tests
{
    [TestClass]
    public class ProgramTests
    {
        [TestMethod]
        public void ProgramRegisteringDependenciesTest()
        {
            //arrange
            var appSettings = new AppSettings
            {
                UnreleasedChangeLogFolderPath = "uhdfhiuhsd"
            };

            var serviceBuilder = new ServiceCollection();
            serviceBuilder.RegisteringDependencies();
            serviceBuilder.AddSingleton<IAppSettings>(appSettings);
            var serviceProvider = serviceBuilder.BuildServiceProvider();
            
            // act and assert
            Assert.IsInstanceOfType(serviceProvider.GetService<IReleaser>(), typeof(Releaser));
            Assert.IsInstanceOfType(serviceProvider.GetService(typeof(IEntryCreator)), typeof(EntryCreator));
            Assert.IsInstanceOfType(serviceProvider.GetService(typeof(IConsoleHelper)), typeof(ConsoleHelper));
            Assert.IsInstanceOfType(serviceProvider.GetService(typeof(IFileWrapper)), typeof(FileWrapper));
            Assert.IsInstanceOfType(serviceProvider.GetService(typeof(IDirectoryService)), typeof(DirectoryService));
            Assert.IsInstanceOfType(serviceProvider.GetService(typeof(IGitHelper)), typeof(GitHelper));
            Assert.IsInstanceOfType(serviceProvider.GetService(typeof(IReleaseManager)), typeof(ReleaseManager));
            Assert.IsInstanceOfType(serviceProvider.GetService(typeof(IDirectoryInfoWrapper)), typeof(DirectoryInfoWrapper));
            Assert.IsInstanceOfType(serviceProvider.GetService(typeof(IFileHelper)), typeof(FileHelper));
            Assert.IsInstanceOfType(serviceProvider.GetService(typeof(ICliExecutor)), typeof(CliExecutor));
            Assert.IsInstanceOfType(serviceProvider.GetService(typeof(IAppSettings)) , typeof(AppSettings));
        }
    }
}
