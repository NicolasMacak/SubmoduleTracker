namespace SubmoduleTracker.Core.GitInteraction.CLI;
public static class GitFacade
{ 
    public static void FetchAndPull(string path)
    {
        GitProcessExecutor.ExecuteVoidCommand(path, "fetch");
        GitProcessExecutor.ExecuteVoidCommand(path, "pull");
    }

    public static void AddAndCommit(string path, string submoduleName)
    {
        GitProcessExecutor.ExecuteVoidCommand(path, $"add {submoduleName}");
        GitProcessExecutor.ExecuteVoidCommand(path, $"commit -m \"{submoduleName}: Automatic forward \"");
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

    private static bool AreChangesInWorkdir(string path)
    {
        // -s for short. Returns just modified files
        string modifiedFilesString = GitProcessExecutor.ExecuteResponseCommand(path, "status -s");

        // no modified fiels
        return modifiedFilesString != string.Empty;
    }

    /// <summary>
    /// Stashes changes
    /// </summary>
    /// <param name="path">Directory to stash changse in</param>
    /// <returns>Return true if some changes were stashed, false otherwise</returns>
    public static bool StashChanges(string path)
    {
        bool areChanges = AreChangesInWorkdir(path);

        if (areChanges) {
            GitProcessExecutor.ExecuteVoidCommand(path, "stash");
            return true;
        }

        return false;
    }

    public static void StashPop(string path)
    {
        GitProcessExecutor.ExecuteVoidCommand(path, "stash pop");
    }
}