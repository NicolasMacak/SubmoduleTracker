using SubmoduleTracker.Core.GitInteraction.Model;

namespace SubmoduleTracker.Core.GitInteraction.Extensions;
public static class Extensions
{
    public static List<string> GetRemotes(this List<GitBranch> gitBranches)
    {
        return gitBranches.Select(x => x.RemoteName).ToList();
    } 
}

