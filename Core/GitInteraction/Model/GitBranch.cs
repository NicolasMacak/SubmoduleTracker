using SubmoduleTracker.Domain.AlignmentExecution;

namespace SubmoduleTracker.Core.GitInteraction.Model;
/// <summary>
/// Contains origin and local name for git branch
/// </summary>
/// <remarks>
/// This exists, because to ease the work with branches. Generally we work with remote names, but in <see cref="AlignmentExecutionWorkflow"/> we work with local names.
/// </remarks>
public class GitBranch(string localName)
{
    /// <summary>
    /// Used when creating commits locally
    /// </summary>
    public readonly string LocalName = localName;
    /// <summary>
    /// Used to inspect and show results of the alignments. We care about the remote state, because that is the state for every developer
    /// </summary>
    public readonly string RemoteName = $"origin/{localName}";
}

