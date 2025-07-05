using LibGit2Sharp;
using SubmoduleTracker.Model;

namespace SubmoduleTracker.Extensions;
public static class RepositoryExtensions
{
    public static IEnumerable<Branch> GetRelevantLocalBranches(this Repository repository, string[] relevantBranches)
    {
        return repository
            .Branches
            .Where(x => relevantBranches.Contains(x.FriendlyName));
    }

    public static IEnumerable<Branch> GetRelevantRemoteBranches(this Repository repository, string[] relevantBranches)
    {
        return repository
            .Branches
            .Where(x => x.IsRemote)
            .Where(x => relevantBranches.Contains(x.FriendlyName.Split("/").Last()));
    }

}