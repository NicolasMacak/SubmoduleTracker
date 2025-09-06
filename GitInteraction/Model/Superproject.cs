using LibGit2Sharp;
using SubmoduleTracker.GitInteraction.CLI;

namespace SubmoduleTracker.GitInteraction.Model;

public sealed class SuperProject
{
    public readonly string Name;
    public readonly string WorkingDirectory;
    public List<string> SubmodulesNames { get; } = new();

    public SuperProject(string repoPath)
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
    /// 
    /// <remarks>
    /// branch - branch for which we want to find out submodule commit indexes <br></br>
    /// indexCommitId - commit to which submodule points to
    /// </remarks>
    /// 
    /// <returns>
    /// Dictionary[string, Dictionary[string, string]] <br></br>
    /// Dictionary[branch, Dictionary[submodule, indexCommitId]]
    /// </returns>

    public async Task<Dictionary<string, Dictionary<string, string>>> GetSubmoduleIndexCommitsRefs(IEnumerable<string> branches, List<string> relevantSubmodueles)
    {
        Dictionary<string, Dictionary<string, string>> result = new();

        foreach (string branch in branches)
        {
            await GitCLI.Switch(WorkingDirectory, branch); // done so we can check where submodules points on this branch

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
    public async Task<Dictionary<string, Dictionary<string, string>>> GetSubmoduleHeadCommitRefs(List<string> relevantBranches, List<string> relevantSubmodules)
    {
        Dictionary<string, Dictionary<string, string>> SubmoduleHeadCommitsForBranches = new();

        foreach (string branchName in relevantBranches)
        {
            Dictionary<string, string> commitMap = new();

            foreach (string submoduleName in relevantSubmodules)
            {
                string submoduleWorkdir = $"{WorkingDirectory}/{submoduleName}";

                await GitCLI.FetchAndPull(submoduleWorkdir); // fetch actual remote state for submodule in question

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