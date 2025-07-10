using LibGit2Sharp;
using SubmoduleTracker.CLI;

namespace SubmoduleTracker.Model;
public sealed class SuperProject : SlimRepository
{
    /// <summary>
    /// Submodules names
    /// </summary>
    public List<string> SubmodulesNames { get; } = new();

    public SuperProject(string repoPath)
        : base(repoPath)
    {
        Repository fullRepo = new (repoPath);

        foreach(var submodule in fullRepo.Submodules)
        {
            SubmodulesNames.Add(submodule.Name);
        }
    }

    /// <summary>
    /// Gets where submodule pointings of superProject for DEV and TEST
    /// </summary>
    public async Task<BranchSubmoduleMap> GetSubmodulePointings(IEnumerable<string> branches)
    {
        BranchSubmoduleMap submodulePoingns = new();

        foreach (string branch in branches)
        {
            await GitCLI.Checkout(RepositoryPath, branch); // done so we can check where submodules points on this branch

            Repository superProjectGitRepository = new(RepositoryPath);

            SubmoduleCommitMap commitMap = new();

            foreach (Submodule? submodule in superProjectGitRepository.Submodules)
            {
                commitMap.Add(submodule.Name, submodule.IndexCommitId.ToString());
            }

            submodulePoingns.Add(branch, commitMap);
        }

        return submodulePoingns;
    }
}