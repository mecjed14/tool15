
namespace Buhler.IoT.Environment.ChangeLogTool.Helper
{
    public interface IGitHelper
    {
        string GetCurrentBranchName();
        string[] ListBranches();
        void AddFileAndCommit(string path);
        string CheckBranchesAndCreate(string branchName);
        void CreateTag(string versionString);
        void PushToRemote(string versionString);
        bool CheckForNoChanges();
        bool CheckOnMasterAndNoChanges();
    }
}
