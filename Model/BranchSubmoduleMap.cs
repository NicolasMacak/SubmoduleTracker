namespace SubmoduleTracker.Model;
public sealed class BranchSubmoduleMap : Dictionary<string, SubmoduleCommitMap>
{
    public SubmoduleCommitMap? GetSubmoduleCommit(string branchName)
    {
        return this.TryGetValue(branchName, out var commit) ? commit : null;
    }
}