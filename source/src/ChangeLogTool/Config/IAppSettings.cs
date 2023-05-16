namespace Buhler.IoT.Environment.ChangeLogTool.Config
{
    public interface IAppSettings
    {
        string MasterChangeLogFileFullPath { get; }
        string MasterChangeLogFilePath { get; set; }
        string UnreleasedChangeLogFolderFullPath { get; }
        string UnreleasedChangeLogFolderPath { get; set; }
        string GetWorkingRootDir();
    }
}
