using SubmoduleTracker.Core.GitInteraction.CommandExceptions;
using SubmoduleTracker.Domain.AlignmentExecution.Exceptions;

namespace SubmoduleTracker.Core.GitInteraction.CLI;
/// <summary>
/// Utility git commands
/// </summary>
public static class GitCommandExecutor
{ 
    public static void FetchAndFastForwardPull(string path)
    {
        ProcessExecutor.ExecuteVoidCommand(path, "fetch");
        try
        {
            ProcessExecutor.ExecuteVoidCommand(path, "pull --ff-only");
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
        ProcessExecutor.ExecuteVoidCommand(path, "fetch --all --recurse-submodules");
        ProcessExecutor.ExecuteVoidCommand(path, "submodule update --init --recursive");
    }

    public static void CreateForwardCommit(string path, string submoduleName)
    {
        ProcessExecutor.ExecuteVoidCommand(path, $"add {submoduleName}");
        try
        {
            ProcessExecutor.ExecuteVoidCommand(path, $"commit -m \"{submoduleName}: Automatic forward\"");
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
        ProcessExecutor.ExecuteVoidCommand(path, "push");
    }

    public static void Switch(string path, string branch)
    {
        string actualBranch = GetCurrentBranch(path);

        // Already on the branch
        if (actualBranch == branch)
        {
            return;
        }

        ProcessExecutor.ExecuteVoidCommand(path, $"switch {branch}");
        ProcessExecutor.ExecuteVoidCommand(path, "submodule update --init --recursive");
    }

    public static string GetCurrentBranch(string path)
    {
        // --show-current branch name is returned
        return ProcessExecutor.ExecuteResponseCommand(path, "branch --show-current");
    }
}