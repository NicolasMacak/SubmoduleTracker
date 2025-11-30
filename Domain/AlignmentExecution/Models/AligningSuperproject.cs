namespace SubmoduleTracker.Domain.AlignmentExecution.Models;
/// <summary>
/// Superproject for which is about to be aligned(Dissaglinment was detected)
/// </summary>
public class AligningSuperproject
{
    /// <param name="Title">Well. Title.</param>
    /// <param name="Workdir">Working directory. Used for executing git commands at specific path</param>
    /// <param name="BranchesToAlign">NON-REMOTE friendly name for branch. Non-remote, because it will be used to create commits</param>

    public readonly string Title;
    /// <summary>
    /// Used for executing git commands at specific path
    /// </summary>
    public readonly string Workdir;
    /// <summary>
    /// NON-REMOTE friendly name for branch. Non-remote, because it will be used to create commits
    /// </summary>
    public readonly List<string> BranchesToAlign;

    public AligningSuperproject(string title, string workdir, List<string> branchesToAlign)
    {
        Title = title;
        Workdir = workdir;
        BranchesToAlign = branchesToAlign;
    }
}