namespace SubmoduleTracker.GitInteraction.Model;
/// <summary>
/// Creating them is expensive. Dont use unless you needs index and head commits obtained through <br></br> 
/// - <see cref="MetaSuperProject.GetSubmoduleHeadCommitRefs(List{string}, List{string})"/> <br></br>
/// - <see cref="MetaSuperProject.GetSubmoduleHeadCommitRefs(List{string}, List{string})"/>
/// </summary>
public sealed class RobustSuperProject
{
    public readonly string Name;
    public readonly string WorkingDirectory;

    /// <summary>
    /// Holds information where which commits submodules of this superprojects points to on relevant branches <br></br>
    /// 
    /// </summary>
    /// <remarks>
    /// Dictionary[branch, Dictionary[submodule, indexCommitId]]
    /// </remarks>
    public Dictionary<string, Dictionary<string, string>> IndexCommitRefs;

    /// <summary>
    /// Holds information where which commits submodules of this superprojects points to on relevant branches <br></br>
    /// </summary>
    /// <remarks>
    /// Dictionary[branch, Dictionary[submodule, headCommitId]]
    /// </remarks>
    public Dictionary<string, Dictionary<string, string>> HeadCommitRefs;

    public RobustSuperProject(
        string name,
        string workingDirectory,
        Dictionary<string, Dictionary<string, string>> indexCommitRefs,
        Dictionary<string, Dictionary<string, string>> headCommitRefs
        )
    {
        Name = name;
        WorkingDirectory = workingDirectory;
        IndexCommitRefs = indexCommitRefs;
        HeadCommitRefs = headCommitRefs;
    }

    /// <summary>
    /// Extract branches from the result collection <see cref="IndexCommitRefs"/>
    /// </summary>
    /// <remarks>
    /// We can use <see cref="IndexCommitRefs"/> and <see cref="HeadCommitRefs"/> interchangably.
    /// Collections has identical structure. [same, [same, different]]
    /// </remarks>
    private IEnumerable<string> GetAvailableBranches()
    {
        return IndexCommitRefs.Keys; // Keys of this dictionary is List of branches
    }

    /// <summary>
    /// Extract submodules from the result collection <see cref="IndexCommitRefs"/>
    /// </summary>
    /// <remarks>
    /// We can use <see cref="IndexCommitRefs"/> and <see cref="HeadCommitRefs"/> interchangably.
    /// Collections has identical structure. [same, [same, different]]
    /// </remarks>
    private IEnumerable<string> GetAvailableSubmodules()
    {
        return IndexCommitRefs
            .First() // Keypair[branch,[submodule, commit]]
            .Value // Keypair[submodule, commit]
            .Select(x => x.Key); // Keys are submoduels names
    }

    /// <summary>
    /// Returns 
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// Dictionary[branch, submodule]
    /// </remarks>
    public Dictionary<string, string> GetDisalignemnts(List<string>? relevantBranches = null, List<string>? relevantSubmodules = null)
    {
        // [branch, submodule]
        Dictionary<string, string> dissalignments = new();

        IEnumerable<string> branchesToIterate = relevantBranches == null
            ? GetAvailableBranches()
            : GetAvailableBranches().Where(relevantBranches.Contains);


        IEnumerable<string> submodulesToIterate = relevantBranches == null
            ? GetAvailableSubmodules()
            : GetAvailableSubmodules().Where(relevantSubmodules!.Contains);

        //IEnumerable<string> branchesToIterate = relevantBranches != null
        //    ? branchesToIterate = relevantBranches.Where(IndexCommitRefs.ContainsKey) // only existing branches
        //    : IndexCommitRefs.Keys; // all branches

        //IEnumerable<string> allSubmodulesInSuperproject = IndexCommitRefs
        //    .First() // Definitely exits. Otherwise superprojct doens't have submodules
        //    .Value // Dictionary[submodule, headCommitId]
        //    .Select(x => x.Key);

        //List<string> submoduelsToIterate = relevantSubmoduels != null
        //    ? allSubmodulesInSuperproject.Where(x => relevantSubmoduels!.Contains(x)).ToList()
        //    : allSubmodulesInSuperproject.ToList();

        foreach (string branch in branchesToIterate)
        {
            foreach (string submodule in submodulesToIterate)
            {
                string indexCommit = IndexCommitRefs[branch][submodule];
                string headCommit = HeadCommitRefs[branch][submodule];

                // Dissaligned
                if(indexCommit != headCommit)
                {
                    dissalignments.Add(indexCommit, headCommit);
                }
            }
        }

        return dissalignments;
    }
}