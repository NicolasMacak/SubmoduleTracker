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

    public static List<SlimRepository> GetRelevantSubmodules(this Repository repo, string[] relevantBranches)
    {
        var submodules = new List<SlimRepository>();
        foreach (var submodule in repo.Submodules)
        {
            var submoduleRepo = new Repository(repo.Info.WorkingDirectory + submodule.Path);
            submodules.Add(new SlimRepository(submoduleRepo.Info.WorkingDirectory, relevantBranches));
        }
        return submodules;
    }
}