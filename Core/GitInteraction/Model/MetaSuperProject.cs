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
    public List<string> SubmodulesNames { get; } = new(); // todo. Get can be removed

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
    /// <param name="branches">branches to get results for</param>
    /// <param name="relevantSubmodules">subomdules to get results for</param>
    /// <remarks>
    /// branch - branch for which we want to find out submodule commit indexes <br></br>
    /// indexCommitId - commit to which submodule points to
    /// </remarks>
    /// 
    /// <returns>
    /// Dictionary[string, Dictionary[string, string]] <br></br>
    /// Dictionary[branch, Dictionary[submodule, indexCommitId]]
    /// </returns>
    
    private Dictionary<string, Dictionary<string, string>> GetSubmoduleIndexCommitsRefs(IEnumerable<string> branches, List<string> relevantSubmodules)
    {
        Dictionary<string, Dictionary<string, string>> pointingsOfSubmodulesForBranches = new(); // Tu isto nechceme tiez remote branches ako v GetSubmoduleHeadCommitRefs??

        Repository mainRepo = new(WorkingDirectory);
        foreach (Branch branch in mainRepo.Branches.Where(x => branches.Contains(x.FriendlyName)))
        {
            Commit headOfBranch = branch.Tip;

            Dictionary<string, string> submodulesPointings = headOfBranch.Tree
                .Where(entry => entry.TargetType == TreeEntryTargetType.GitLink) // GitLink => Submodule
                .ToDictionary(entry => entry.Name, entry => entry.Target.Id.ToString()[..20]); // [SubmoduleName, IndexCommit(First 20 chars)]

            
            pointingsOfSubmodulesForBranches.Add(branch.FriendlyName, submodulesPointings );
        }

        //foreach (string branchName in branches)
        //{
        //    GitFacade.Switch(WorkingDirectory, branchName); // done so we can check where submodules points on this branch

        //    Repository superProjectGitRepository = new(WorkingDirectory); // Load superproject where files were alteredy by checkout

        //    // Information where submodules points to
        //    Dictionary<string, string> submoduleCommitIndexes 
        //        = superProjectGitRepository.Submodules
        //        .Where(x => relevantSubmodules.Contains(x.Name))
        //        .ToDictionary(x => x.Name, x => x.IndexCommitId.ToString()[..20]); // [..20] - first 20 chars

        //    result.Add(branchName, submoduleCommitIndexes);
        //}

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
        Dictionary<string, Dictionary<string, string>> SubmoduleHeadCommitsForBranches = new();

        // Dictionary[string, Dictionary[string, string]] < br ></ br > Can be Dictionary[string, KeyValuePair[string, string]] <br></br>
        Dictionary<string, KeyValuePair<string, string>> Experimental = new();

        List<string> remoteRelevantBranchesNames = relevantBranches.Select(x => $"origin/{x}").ToList(); // ensures that later we consider only remote branches

        foreach(string submoduleName in relevantSubmodules)
        {
            Repository submoduleRepo = new($@"{WorkingDirectory}\{submoduleName}");

            IEnumerable<Branch> remoteRelevantBranches = submoduleRepo.Branches
                .Where(x => remoteRelevantBranchesNames.Contains(x.FriendlyName));

            foreach (Branch branch in remoteRelevantBranches)
            {
                string remoteHeadCommit = branch.Tip.Sha[..20]; // Hash of head commit

                Experimental[branch.FriendlyName.Split("/").Last()] = new KeyValuePair<string, string>(submoduleName, remoteHeadCommit);
            }
        }

        //foreach (string branchName in relevantBranches)
        //{
        //    Dictionary<string, string> commitMap = new();

        //    foreach (string submoduleName in relevantSubmodules)
        //    {
        //        string submoduleWorkdir = $@"{WorkingDirectory}\{submoduleName}";

        //        GitFacade.Switch(submoduleWorkdir, branchName); // done so we can check where submodules points on this branch
        //        GitFacade.FetchAndPull(submoduleWorkdir); // fetch actual remote state for submodule in question

        //        Repository submoduleRepository = new(submoduleWorkdir);

        //        Branch? branch = submoduleRepository.Branches.FirstOrDefault(x => x.IsRemote && x.FriendlyName.EndsWith(branchName)); // find 
        //        if (branch == null)
        //        {
        //            Console.WriteLine($"{branch} was not found in {submoduleWorkdir}");
        //        }

        //        commitMap.Add(submoduleName, branch!.Reference.TargetIdentifier[..20]); // [..20] - first 20 chars
        //    }

        //    SubmoduleHeadCommitsForBranches.Add(branchName, commitMap);
        //}

        return SubmoduleHeadCommitsForBranches;
    }        

    public RobustSuperProject ToRobustSuperproject(List<string> relevantBranches, List<string>? relevantSubmodules = null)
    {
        CustomConsole.WriteLineColored($"Superproject: {Name} >> Fetching Index Commits and Head Commits", ConsoleColor.DarkCyan);
        GitFacade.FetchAllInMainAndSubmodules(WorkingDirectory);

        return new RobustSuperProject(
            name: Name,
            workingDirectory: WorkingDirectory,
            submodulesNames: SubmodulesNames,
            indexCommitRefs: GetSubmoduleIndexCommitsRefs(relevantBranches, relevantSubmodules ?? SubmodulesNames),
            headCommitRefs: GetSubmoduleHeadCommitRefs(relevantBranches, relevantSubmodules ?? SubmodulesNames)
        );
    }
}