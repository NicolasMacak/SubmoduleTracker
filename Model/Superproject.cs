using LibGit2Sharp;
using SubmoduleTracker.CLI;

namespace SubmoduleTracker.Model;
public sealed class SuperProject : SlimRepository
{
    //public readonly List<SlimRepository> Submodules;

    public BranchSubmoduleMap SubmoduleMap { get; } = new();

    public List<string> SubmodulesNames { get; } = new();

    // Dictionary<branch <submodule, commit>>

    // kam ukazuje local branch? easy
    // Kam ukazuje remote branch? musim byt na head commit tej ktorej branche

    public SuperProject(string repoPath)
        : base(repoPath)
    {
        var fullRepo = new Repository(repoPath);

        foreach(var submodule in fullRepo.Submodules)
        {
            SubmodulesNames.Add(submodule.Name);
        }
    }

    public async Task PopulateSubmoduleReferencesForBranches(IEnumerable<string> branches)
    {
        foreach (string branch in branches)
        {
            await GitCLI.Checkout(RepositoryPath, branch);
            SaveSubmoduleRefernceForBranch(branch);
        }
    }

    private void SaveSubmoduleRefernceForBranch(string branch)
    {
        Repository repositoryData = new(RepositoryPath);

        SubmoduleCommitMap commitMap = new ();

        foreach (Submodule? submodule in repositoryData.Submodules)
        {
            commitMap.Add(submodule.Name, submodule.IndexCommitId.ToString());
        }

        SubmoduleMap.Add(branch, commitMap);
    }
}