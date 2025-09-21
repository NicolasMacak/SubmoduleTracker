namespace SubmoduleTracker.Domain.SubmoduleIndexValidation.Dto;

[Obsolete("Use robust superproject")]
public sealed class PrintableSuperprojectDto
{
    public string Title { get; set; }
    public List<string> RevelantBranches { get; set; }
    public List<string> Submodules { get; set; }
    public Dictionary<string, Dictionary<string, string>> SubmoduleCommitIndexes { get; set; }
    public Dictionary<string, Dictionary<string, string>> SubmodulesHeadCommits { get; set; }
}