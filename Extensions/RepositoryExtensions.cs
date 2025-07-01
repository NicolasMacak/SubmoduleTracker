using LibGit2Sharp;

namespace SubmoduleTracker.Extensions;
public static class RepositoryExtensions
{
    public static IEnumerable<Branch> GetLocalBranches(this Repository repository)
    {
        return repository
            .Branches
            .Where(x => !x.IsRemote);
    }

    public static IEnumerable<Branch> GetRemoteBranches(this Repository repository)
    {
        return repository
            .Branches
            .Where(x => x.IsRemote);
    }
}