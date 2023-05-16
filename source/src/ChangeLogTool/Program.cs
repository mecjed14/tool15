namespace Buhler.IoT.Environment.ChangeLogTool
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Tools;
    using Config;
    using Microsoft.Extensions.Configuration;
    using System.IO;
    using System.Diagnostics.CodeAnalysis;
    using Helper;
    using Wrapper;

    public static class Program
    {
        [ExcludeFromCodeCoverage]
        private static void Main(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder()
                 .SetBasePath(Path.GetDirectoryName(typeof(Program).Assembly.Location)!)
                 .AddJsonFile("appsettings.json", false);

            var configuration = configurationBuilder.Build();
            var appSettings = configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();

            var host = Host.CreateDefaultBuilder()
              .ConfigureHostConfiguration(c => c.AddConfiguration(configuration))
              .ConfigureServices((_, services) =>
              {
                  services.AddSingleton<IAppSettings>(appSettings);
                  services.RegisteringDependencies();
              })
              .Build();

            var cliExecutor = host.Services.GetService<ICliExecutor>();
            cliExecutor.Execute(args);
        }

        public static void RegisteringDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IReleaser, Releaser>();
            services.AddSingleton<IEntryCreator, EntryCreator>();
            services.AddSingleton<IConsoleHelper, ConsoleHelper>();
            services.AddSingleton<IFileWrapper, FileWrapper>();
            services.AddSingleton<IDirectoryService, DirectoryService>();
            services.AddSingleton<IGitHelper, GitHelper>();
            services.AddSingleton<IReleaseManager, ReleaseManager>();
            services.AddSingleton<IDirectoryInfoWrapper, DirectoryInfoWrapper>();
            services.AddSingleton<IFileHelper, FileHelper>();
            services.AddSingleton<ICliExecutor, CliExecutor>();
        }
    }
}
