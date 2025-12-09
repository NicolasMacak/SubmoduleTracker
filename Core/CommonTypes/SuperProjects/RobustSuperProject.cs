namespace SubmoduleTracker.Core.CommonTypes.SuperProjects;
/// <summary>
/// <see cref="MetaSuperProject"/> enriched by <see cref="IndexCommitRefs" and <see cref="HeadCommitRefs"/>
/// </summary>
public sealed class RobustSuperProject
{
    public readonly string Name;
    public readonly string WorkingDirectory;
    public List<string> SubmodulesNames; 

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
}