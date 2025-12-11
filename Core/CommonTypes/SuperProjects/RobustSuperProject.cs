namespace SubmoduleTracker.Core.CommonTypes.SuperProjects;
/// <summary>
/// <see cref="MetaSuperProject"/> enriched by <see cref="IndexCommitRefs" and <see cref="HeadCommitRefs"/>
/// </summary>
public sealed class RobustSuperProject(
    string name,
    string workingDirectory,
    List<string> submodulesNames,
    Dictionary<string, Dictionary<string, string>> indexCommitRefs,
    Dictionary<string, Dictionary<string, string>> headCommitRefs
    )
{
    public readonly string Name = name;
    /// <summary>
    /// Absolute path to the Git repository in file system
    /// </summary>
    public readonly string WorkingDirectory = workingDirectory;
    public List<string> SubmodulesNames = submodulesNames; 

    /// <summary>
    /// Holds information where which commits submodules of this superprojects points to on relevant branches <br></br>
    /// </summary>
    /// <remarks>
    /// Dictionary[branch, Dictionary[submodule, indexCommitId]]
    /// </remarks>
    public Dictionary<string, Dictionary<string, string>> IndexCommitRefs = indexCommitRefs;


    /// <summary>
    /// Holds information where which commits submodules of this superprojects points to on relevant branches <br></br>
    /// </summary>
    /// <remarks>
    /// Dictionary[branch, Dictionary[submodule, headCommitId]]
    /// </remarks>
    public Dictionary<string, Dictionary<string, string>> HeadCommitRefs = headCommitRefs;
}