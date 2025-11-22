using LibGit2Sharp;
using SubmoduleTracker.Core.ConsoleTools;
using SubmoduleTracker.Core.GitInteraction.CLI;

namespace SubmoduleTracker.Core.GitInteraction.Model;

/// <summary>
/// Has metadata to obtain informations
/// </summary>
/// <remarks>
/// Doesn't hold any git data information(time expensive), only has the means to obtain them.
/// We would need to load data for all relevant branches and all submodules. <br></br>
/// <see cref="ToRobustSuperproject"/> will create with object with relevant data when they are needed
/// </remarks>

// ConfigSuperProject - workdir
// MetaSuperProject - + Submodules
// RobustSuperProject - + Commit pointings

public sealed class MetaSuperProject
{
    public readonly string Name;
    public readonly string WorkingDirectory;
    public List<string> SubmodulesNames { get; } = new();

    public MetaSuperProject(string repoPath)
    {
        Name = repoPath.Split(@"\").Last();
        WorkingDirectory = repoPath;

        Repository fullRepo = new(repoPath);
        foreach (var submodule in fullRepo.Submodules)
        {
            SubmodulesNames.Add(submodule.Name);
        }
    }

    /// <summary>
    /// Get commit ids to which submodules points to for provided branches (DEV, TEST)
    /// </summary>
    /// <param name="relevantBranches">branches to get results for</param>
    /// <remarks>
    /// branch - branch for which we want to find out submodule commit indexes <br></br>
    /// indexCommitId - commit to which submodule points to
    /// Dictionary[string, Dictionary[string, string]] <br></br>
    /// Dictionary[branch, Dictionary[submodule, indexCommitId]]
    /// </returns>
    
    private Dictionary<string, Dictionary<string, string>> GetSubmoduleIndexCommitsRefs(IEnumerable<string> relevantBranches)
    {
        Dictionary<string, Dictionary<string, string>> pointingsOfSubmodulesForBranches = new();
        List<string> remoteRelevantBranchesNames = relevantBranches.Select(x => $"origin/{x}").ToList(); // Remote branches used so we can get data when branches doesn't exist locally

        Repository mainRepo = new(WorkingDirectory);

        foreach (Branch branch in mainRepo.Branches.Where(x => remoteRelevantBranchesNames.Contains(x.FriendlyName)))
        {
            Commit headOfBranch = branch.Tip;

            Dictionary<string, string> submodulesPointings = headOfBranch.Tree
                .Where(entry => entry.TargetType == TreeEntryTargetType.GitLink) // GitLink => Submodule
                .ToDictionary(entry => entry.Name, entry => entry.Target.Id.ToString()[..20]); // [SubmoduleName, IndexCommit(First 20 chars)]

            string friendlyName = branch.FriendlyName.Split("/").Last(); // remove prefix "origin/"
            pointingsOfSubmodulesForBranches.Add(friendlyName , submodulesPointings);
        }

        return pointingsOfSubmodulesForBranches;
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
    private Dictionary<string, Dictionary<string, string>> GetSubmoduleHeadCommitRefs(List<string> relevantBranches, List<string> relevantSubmodules)
    {
        Dictionary<string, Dictionary<string, string>> submoduleHeadCommitsForBranches = new();

        List<string> remoteRelevantBranchesNames = relevantBranches.Select(x => $"origin/{x}").ToList(); // Remote branches used so we can get data when branches doesn't exist locally

        foreach (string submoduleName in relevantSubmodules)
        {
            Repository submoduleRepo = new($@"{WorkingDirectory}\{submoduleName}");

            List<Branch> remoteRelevantBranches = submoduleRepo.Branches
                .Where(x => remoteRelevantBranchesNames.Contains(x.FriendlyName)).ToList();

            foreach (Branch branch in remoteRelevantBranches)
            {
                string remoteHeadCommit = branch.Tip.Sha[..20];
                string branchFriendlyName = branch.FriendlyName.Split("/").Last(); // remove prefix "origin/"

                if (submoduleHeadCommitsForBranches.TryGetValue(branchFriendlyName, out Dictionary<string, string>? value))
                {
                    value.Add(submoduleName, remoteHeadCommit); // Key exists. Add an item to dictionary attached to it
                }
                else
                {
                    submoduleHeadCommitsForBranches[branchFriendlyName] = new() { { submoduleName, remoteHeadCommit } }; // Key doesnt exists. Initiate dictionary with item
                }
            }
        }

        return submoduleHeadCommitsForBranches;
    }        

    public RobustSuperProject ToRobustSuperproject(List<string> relevantBranches, List<string>? relevantSubmodules = null)
    {
        CustomConsole.WriteLineColored($"Superproject: {Name} >> Fetching Index Commits and Head Commits", ConsoleColor.DarkCyan);
        GitFacade.FetchAllInMainAndSubmodules(WorkingDirectory);

        return new RobustSuperProject(
            name: Name,
            workingDirectory: WorkingDirectory,
            submodulesNames: SubmodulesNames,
            indexCommitRefs: GetSubmoduleIndexCommitsRefs(relevantBranches),
            headCommitRefs: GetSubmoduleHeadCommitRefs(relevantBranches, relevantSubmodules ?? SubmodulesNames)
        );
    }
}