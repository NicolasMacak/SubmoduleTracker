using LibGit2Sharp;
using SubmoduleTracker.GitInteraction.CLI;

namespace SubmoduleTracker.GitInteraction.Model;

/// <summary>
/// Has metadata to obtain informations
/// </summary>
/// <remarks>
/// Executing methods listed bellow is very time expensive. So this model has only means to it.
/// <see cref="RobustSuperProject"/> is created when those data are needed. <br></br>
/// - <see cref="GetSubmoduleIndexCommitsRefs(IEnumerable{string}, List{string})"/> <br></br>
/// - <see cref="GetSubmoduleHeadCommitRefs(List{string}, List{string})"/> 
/// </remarks>
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
    /// <param name="relevantSubmodueles">subomdules to get results for</param>
    /// <remarks>
    /// branch - branch for which we want to find out submodule commit indexes <br></br>
    /// indexCommitId - commit to which submodule points to
    /// </remarks>
    /// 
    /// <returns>
    /// Dictionary[string, Dictionary[string, string]] <br></br>
    /// Dictionary[branch, Dictionary[submodule, indexCommitId]]
    /// </returns>

    public Dictionary<string, Dictionary<string, string>> GetSubmoduleIndexCommitsRefs(IEnumerable<string> branches, List<string> relevantSubmodueles)
    {
        Dictionary<string, Dictionary<string, string>> result = new();

        foreach (string branch in branches)
        {
            GitFacade.Switch(WorkingDirectory, branch); // done so we can check where submodules points on this branch

            Repository superProjectGitRepository = new(WorkingDirectory); // Load superproject where files were alteredy by checkout

            // Information where submodules points to
            Dictionary<string, string> submoduleCommitIndexes 
                = superProjectGitRepository.Submodules
                .Where(x => relevantSubmodueles.Contains(x.Name))
                .ToDictionary(x => x.Name, x => x.IndexCommitId.ToString()[..20]); // [..20] - first 20 chars

            result.Add(branch, submoduleCommitIndexes);
        }

        return result;
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
    public Dictionary<string, Dictionary<string, string>> GetSubmoduleHeadCommitRefs(List<string> relevantBranches, List<string> relevantSubmodules)
    {
        Dictionary<string, Dictionary<string, string>> SubmoduleHeadCommitsForBranches = new();

        foreach (string branchName in relevantBranches)
        {
            Dictionary<string, string> commitMap = new();

            foreach (string submoduleName in relevantSubmodules)
            {
                string submoduleWorkdir = $"{WorkingDirectory}/{submoduleName}";

                GitFacade.FetchAndPull(submoduleWorkdir); // fetch actual remote state for submodule in question

                Repository submoduleRepository = new(submoduleWorkdir);

                Branch? branch = submoduleRepository.Branches.FirstOrDefault(x => x.IsRemote && x.FriendlyName.EndsWith(branchName)); // find 
                if (branch == null)
                {
                    Console.WriteLine($"{branch} was not found in {submoduleWorkdir}");
                }

                commitMap.Add(submoduleName, branch!.Reference.TargetIdentifier[..20]); // [..20] - first 20 chars
            }

            SubmoduleHeadCommitsForBranches.Add(branchName, commitMap);
        }

        return SubmoduleHeadCommitsForBranches;
    }        
}