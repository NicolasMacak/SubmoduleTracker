namespace SubmoduleTracker.Core.GitInteraction.Model;
/// <summary>
/// Creating them is expensive. Dont use unless you needs index and head commits obtained through <br></br> 
/// - <see cref="MetaSuperProject.GetSubmoduleHeadCommitRefs(List{string}, List{string})"/> <br></br>
/// - <see cref="MetaSuperProject.GetSubmoduleHeadCommitRefs(List{string}, List{string})"/>
/// </summary>
public sealed class RobustSuperProject
{
    public readonly string Name;
    public readonly string WorkingDirectory;
    public List<string> SubmodulesNames; // exactly as it sounds

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
        List<string> submodulesNames,
        Dictionary<string, Dictionary<string, string>> indexCommitRefs,
        Dictionary<string, Dictionary<string, string>> headCommitRefs
        )
    {
        Name = name;
        WorkingDirectory = workingDirectory;
        SubmodulesNames = submodulesNames;
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
    public IList<string> GetAvailableBranches()
    {
        return IndexCommitRefs
            .Keys
            .ToList(); // Keys of this dictionary is List of branches
    }

    /// <summary>
    /// Extract submodules from the result collection <see cref="IndexCommitRefs"/>
    /// </summary>
    /// <remarks>
    /// We can use <see cref="IndexCommitRefs"/> and <see cref="HeadCommitRefs"/> interchangably.
    /// Collections has identical structure. [same, [same, different]]
    /// </remarks>
    private IList<string> GetAvailableSubmodules()
    {
        return IndexCommitRefs
            .First() // Keypair[branch,[submodule, commit]]
            .Value // Keypair[submodule, commit]
            .Select(x => x.Key) // Keys are submoduels names
            .ToList(); 
    }

    /// <summary>
    /// Returns dictionary stating which submodules are missaligned on which branch 
    /// </summary>
    /// <returns>Dictionary[branch, submodule]. Empty when there are not dissalignments </returns>
    public Dictionary<string, string> GetDisalignemnts()
    {
        // [branch, submodule]
        Dictionary<string, string> dissalignments = new();

        IList<string> branchesToIterate = GetAvailableBranches();
        if (branchesToIterate.Count == 0)
        {
            return dissalignments;
        }

        IList<string> submodulesToIterate = GetAvailableSubmodules();

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