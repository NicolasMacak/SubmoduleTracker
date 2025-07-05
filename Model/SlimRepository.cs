using LibGit2Sharp;
using SubmoduleTracker.Extensions;

namespace SubmoduleTracker.Model;

[Obsolete]
public class SlimRepository
{
    public readonly string Name;
    public readonly string RepositoryPath;
    [Obsolete("needed? Uz mam pointery na submoduly")]
    public readonly List<Branch> RelevantBranches;


    public SlimRepository(string repoPath)
    {
        Name = repoPath.Split(@"\").Last();
        RepositoryPath = repoPath;
        Repository fullRepository = new (repoPath);
    }
}