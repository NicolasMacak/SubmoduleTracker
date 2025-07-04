namespace SubmoduleTracker.Model;

[Obsolete]
public sealed class SubmoduleCustom : SlimRepository
{
    public SubmoduleCustom(string repoPath, string[] relevantBranches)
        : base(repoPath, relevantBranches)
    {
    }
}