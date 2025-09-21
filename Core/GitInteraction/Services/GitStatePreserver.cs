using SubmoduleTracker.Core.ConsoleTools;
using SubmoduleTracker.Core.GitInteraction.CLI;

namespace SubmoduleTracker.Core.GitInteraction.Services;
/// <summary>
/// Remembers branch and changes in modified files. So after alignment, user can be returned to where he was
/// </summary>
public sealed class GitStatePreserver
{
    /// <summary>
    /// Superproject that is being aligned
    /// </summary>
    private readonly string _superProjectWorkdir;
    /// <summary>
    /// Submodule being aligned
    /// </summary>
    private readonly string _submoduleWorkdir;
    /// <summary>
    /// Branch to return to after alignment. Empty if return is not needed
    /// </summary>
    private string _superProjectBranchToReturnTo { get; set; } = string.Empty;

    private bool _wasSuperprojectStashed {  get; set; }

    public GitStatePreserver(string superProjectWorkdir, string submoduleWordir)
    {
        _superProjectWorkdir = superProjectWorkdir;
        _submoduleWorkdir = submoduleWordir;
    }

    /// <summary>
    /// Remembers user's branch and stashed changes in superproject and submodule
    /// </summary>
    /// <returns>True if there were modified files or if user was not on aligning branch. False otherwise</returns>
    public bool SaveState(string aligningBranch) // return value is deciding, whether state needs to be loader later
    {
        // save superprojectBranch
        string currentBranch = GitFacade.GetCurrentBranch(_superProjectWorkdir);
        _superProjectBranchToReturnTo = currentBranch != aligningBranch ? currentBranch : string.Empty;

        _wasSuperprojectStashed = GitFacade.StashChanges(_superProjectBranchToReturnTo);

        bool wasSubmoduleStashed = GitFacade.StashChanges(_submoduleWorkdir);
        // We wont restore submodule state later. We just let the user know
        if (wasSubmoduleStashed)
        {
            CustomConsole.WriteColored($"Your work in submodule {_submoduleWorkdir} was stashed. Review it", ConsoleColor.DarkCyan);
        }

        // We want to restore these changes later
        return _superProjectBranchToReturnTo != string.Empty || _wasSuperprojectStashed;
    }

    public void RestoreState()
    {
        // We return to branch we were at before aligning
        if (!string.IsNullOrEmpty(_superProjectBranchToReturnTo))
        {
            GitFacade.Switch(_submoduleWorkdir, _superProjectBranchToReturnTo);
        }

        // we pop changes from the stash
        if (_wasSuperprojectStashed)
        {
            GitFacade.StashPop(_submoduleWorkdir);
        }
    }
}