using LibGit2Sharp;
using SubmoduleTracker.CLI;

namespace SubmoduleTracker.Managers;

/// <summary>
/// Contains heads commits for all submodules of all relevant branches
/// </summary>
public sealed class SubmodulesManager
{
    private readonly Dictionary<string, string> _allSubmodulesWorkdirs;

    public SubmodulesManager(Dictionary<string, string> submodulesWithPaths)
    {
        _allSubmodulesWorkdirs = submodulesWithPaths;
    }

    /// <summary>
    /// Get HEAD commit indexes for submodules on every branch
    /// </summary>
    /// 
    /// <remarks>
    /// branch - branch for which we want to find out submodule head commits id <br></br>
    /// headCommitId - HEAD commit of submodule for provided branch
    /// </remarks>
    /// 
    /// <returns>
    /// Dictionary[string, Dictionary[string, string]] <br></br>
    /// Dictionary[branch, Dictionary[submodule, HeadCommitId]]
    /// </returns>
    public async Task<Dictionary<string, Dictionary<string, string>>> GetHeadsOfAllSubmodules(List<string> branchNames)
    {
        Dictionary<string, Dictionary<string, string>> SubmoduleHeadCommitsForBranches = new();

        foreach(string branchName in branchNames)
        {
            Dictionary<string, string> commitMap = new ();

            foreach (KeyValuePair<string, string> submoduleWithPath in _allSubmodulesWorkdirs)
            {
                await GitCLI.Fetch(submoduleWithPath.Value); // fetch actual remote state for submodule in question

                Repository submoduleRepository = new(submoduleWithPath.Value);

                Branch? branch = submoduleRepository.Branches.FirstOrDefault(x => x.IsRemote && x.FriendlyName.EndsWith(branchName)); // find 
                if(branch == null)
                {
                    Console.WriteLine($"{branch} was not found in {submoduleWithPath.Key}");
                }

                commitMap.Add(submoduleWithPath.Key, branch!.Reference.TargetIdentifier[..20]);
            }

            SubmoduleHeadCommitsForBranches.Add(branchName, commitMap);
        }

        return SubmoduleHeadCommitsForBranches;
    }
}