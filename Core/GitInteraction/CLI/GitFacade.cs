using SubmoduleTracker.Core.GitInteraction.CommandExceptions;
using SubmoduleTracker.Domain.AlignmentExecution.Exceptions;

namespace SubmoduleTracker.Core.GitInteraction.CLI;
public static class GitFacade
{ 
    public static void FetchAndFastForwardPull(string path)
    {
        GitProcessExecutor.ExecuteVoidCommand(path, "fetch");
        try
        {
            GitProcessExecutor.ExecuteVoidCommand(path, "pull --ff-only");
        }
        catch (CommandExecutionException)
        {
            throw new FastForwardMergeFailureException(path);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public static void FetchAllInMainAndSubmodules(string path)
    {
        GitProcessExecutor.ExecuteVoidCommand(path, "fetch --all --recurse-submodules");
        GitProcessExecutor.ExecuteVoidCommand(path, "submodule update --init --recursive");
    }

    public static void CreateForwardCommit(string path, string submoduleName)
    {
        GitProcessExecutor.ExecuteVoidCommand(path, $"add {submoduleName}");
        try
        {
            GitProcessExecutor.ExecuteVoidCommand(path, $"commit -m \"{submoduleName}: Automatic forward\"");
        }
        catch (CommandExecutionException)
        {
            throw new ForwardCommitCreationException(path);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public static void Push(string path)
    {
        GitProcessExecutor.ExecuteVoidCommand(path, "push");
    }

    public static void Switch(string path, string branch)
    {
        string actualBranch = GetCurrentBranch(path);

        // Already on the branch
        if (actualBranch == branch)
        {
            return;
        }

        GitProcessExecutor.ExecuteVoidCommand(path, $"switch {branch}");
        GitProcessExecutor.ExecuteVoidCommand(path, "submodule update --init --recursive");
    }

    public static string GetCurrentBranch(string path)
    {
        // --show-current branch name is returned
        return GitProcessExecutor.ExecuteResponseCommand(path, "branch --show-current");
    }
}