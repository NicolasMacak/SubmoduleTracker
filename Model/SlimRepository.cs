using LibGit2Sharp;
using SubmoduleTracker.Extensions;

namespace SubmoduleTracker.Model;
public class SlimRepository
{
    public readonly string Name;
    public readonly List<Branch> RelevantBranches;

    public SlimRepository(string repoPath, string[] relevantBranches)
    {
        Name = repoPath.Split(@"\").Last();
        Repository fullRepository = new (repoPath);

        RelevantBranches = fullRepository
            .GetRelevantLocalBranches(relevantBranches)
            .ToList();
    }
}