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
}