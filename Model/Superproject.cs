using LibGit2Sharp;
using SubmoduleTracker.Extensions;

namespace SubmoduleTracker.Model;
public sealed class SuperProject : SlimRepository
{
    //public readonly List<SlimRepository> Submodules;

    public BranchSubmoduleMap SubmoduleMap { get; } = new();

    // Dictionary<branch <submodule, commit>>

    // kam ukazuje local branch? easy
    // Kam ukazuje remote branch? musim byt na head commit tej ktorej branche

    public SuperProject(string repoPath, string[] relevantBranches)
        : base(repoPath, relevantBranches)
    {
    }

    public void AddSubmoduleCommitMapForBranch(string branch)
    {
        Repository fullRepository = new(RepositoryPath);

        SubmoduleCommitMap commitMap = new ();

        foreach (Submodule? submodule in fullRepository.Submodules)
        {
            commitMap.Add(submodule.Name, submodule.IndexCommitId.ToString());
        }

        SubmoduleMap.Add(branch, commitMap);
    }
}