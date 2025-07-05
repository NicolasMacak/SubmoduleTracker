using LibGit2Sharp;
using SubmoduleTracker.CLI;
using SubmoduleTracker.Model;

namespace SubmoduleTracker.Managers;

/// <summary>
/// Contains heads commits for all submodules of all relevant branches
/// </summary>
public sealed class SubmodulesManager
{
    BranchSubmoduleMap BranchAndHeadCommitsOnSubmodules = new();

    private readonly Dictionary<string, string> _submodulesWithPaths;

    public SubmodulesManager(Dictionary<string, string> submodulesWithPaths)
    {
        _submodulesWithPaths = submodulesWithPaths;
    }


    /// <summary>
    /// Zisit kam smeruju HEADy pre jednotlive branche vsetkych submodulov
    /// </summary>
    public async Task GetHeadsOfAllSubmodules(List<string> branchNames)
    {
        foreach(string branchName in branchNames)
        {
            SubmoduleCommitMap commitMap = new ();

            foreach (KeyValuePair<string, string> submoduleWithPath in _submodulesWithPaths)
            {
                await GitCLI.Fetch(submoduleWithPath.Value);

                Repository submoduleRepository = new Repository(submoduleWithPath.Value);

                Branch? branch = submoduleRepository.Branches.FirstOrDefault(x => x.IsRemote && x.FriendlyName.EndsWith(branchName));
                if(branch == null)
                {
                    Console.WriteLine($"{branch} was not found in {submoduleWithPath.Key}");
                }

                commitMap.Add(submoduleWithPath.Key, branch!.Reference.TargetIdentifier);
            }

            BranchAndHeadCommitsOnSubmodules.Add(branchName, commitMap);
        }
    }
}