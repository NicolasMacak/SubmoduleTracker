namespace SubmoduleTracker.Model;
public sealed class Submodule : SlimRepository
{
    public Submodule(string repoPath, string[] relevantBranches)
        : base(repoPath, relevantBranches)
    {
    }
}