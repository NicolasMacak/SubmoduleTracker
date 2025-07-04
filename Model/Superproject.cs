using LibGit2Sharp;
using SubmoduleTracker.Extensions;

namespace SubmoduleTracker.Model;
public sealed class SuperProject : SlimRepository
{
    //public readonly List<SlimRepository> Submodules;

    public BranchSubmoduleMap SubmoduleMap { get; } = new();

    public HashSet<string> SubmodulesNames { get; } = new();

    // Dictionary<branch <submodule, commit>>

    // kam ukazuje local branch? easy
    // Kam ukazuje remote branch? musim byt na head commit tej ktorej branche

    public SuperProject(string repoPath)
        : base(repoPath)
    {
    }

    public void AddSubmoduleCommitMapForBranch(string branch)
    {
        Repository fullRepository = new(RepositoryPath);

        SubmoduleCommitMap commitMap = new ();

        foreach (Submodule? submodule in fullRepository.Submodules)
        {
            SubmodulesNames.Add(submodule.Name);
            commitMap.Add(submodule.Name, submodule.IndexCommitId.ToString());
        }

        SubmoduleMap.Add(branch, commitMap);
    }
}