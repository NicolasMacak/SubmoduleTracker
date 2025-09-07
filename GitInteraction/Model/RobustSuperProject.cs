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
    public List<string> SubmodulesNames;


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
        List<string> submoduleNames,
        Dictionary<string, Dictionary<string, string>> indexCommitRefs,
        Dictionary<string, Dictionary<string, string>> headCommitRefs
        )
    {
        Name = name;
        WorkingDirectory = workingDirectory;
        SubmodulesNames = submoduleNames;
        IndexCommitRefs = indexCommitRefs;
        HeadCommitRefs = headCommitRefs;
    }

    //public static RobustSuperProject FromMetaSuperproject(MetaSuperProject metaSuperProject)
    //{
    //    return new RobustSuperProject(
    //            name: metaSuperProject.Name,
    //            workingDirectory: metaSuperProject.WorkingDirectory,
    //            submoduleNames: metaSuperProject.SubmodulesNames,
    //            indexCommitRefs: metaSuperProject.
    //        )
    //}

}