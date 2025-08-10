using LibGit2Sharp;
using SubmoduleTracker.CLI;

namespace SubmoduleTracker.Model;

public sealed class SuperProject 
{
    public readonly string Name;
    public readonly string RepositoryPath;
    public List<string> SubmodulesNames { get; } = new();

    /// <summary>
    /// Information where submodules points to. Generated for every branch
    /// </summary>
    public Dictionary<string, Dictionary<string, string>> SubmoduleCommitIndexesForBranches = new();

    public SuperProject(string repoPath)
    {
        Name = repoPath.Split(@"\").Last();
        RepositoryPath = repoPath;

        Repository fullRepo = new (repoPath);
        foreach(var submodule in fullRepo.Submodules)
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

    public async Task FetchLatestSubmodulesIndexCommits(IEnumerable<string> branches)
    {
        foreach (string branch in branches)
        {
            await GitCLI.Checkout(RepositoryPath, branch); // done so we can check where submodules points on this branch

            Repository superProjectGitRepository = new(RepositoryPath); // Load superproject where files were alteredy by checkout

            //Dictionary<string, string> commitMap = new();
            //foreach (Submodule? submodule in superProjectGitRepository.Submodules) // todo. This can probably be constructed by LINQ
            //{
            //    commitMap.Add(submodule.Name, submodule.IndexCommitId.ToString());
            //}

            Dictionary<string, string> submoduleCommitIndexes = superProjectGitRepository.Submodules.ToDictionary(x => x.Name, x => x.IndexCommitId.ToString()); // Information where submodules points to

            SubmoduleCommitIndexesForBranches.Add(branch, submoduleCommitIndexes);
        }
    }
}