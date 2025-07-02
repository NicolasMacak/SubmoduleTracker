using LibGit2Sharp;
using SubmoduleTracker.Extensions;

namespace SubmoduleTracker.Model;
public sealed class Superproject : SlimRepository
{
    public readonly List<SlimRepository> Submodules;

    public Superproject(string repoPath, string[] relevantBranches)
        : base(repoPath, relevantBranches)
    {
        Repository fullRepository = new(repoPath);

        Submodules = fullRepository.GetRelevantSubmodules(relevantBranches);
    }
}