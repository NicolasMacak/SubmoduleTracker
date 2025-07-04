namespace SubmoduleTracker.Model;

/// <summary>
/// Dictionary<Submodule, CommitIndex>>
/// </summary>

public sealed class SubmoduleCommitMap : Dictionary<string, string>
{
    public string GetPointingCommitOfSubmodule (string submoduleName)
    {
        return this.TryGetValue(submoduleName, out var commit) ? commit : string.Empty;
    }
}