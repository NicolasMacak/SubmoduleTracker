namespace SubmoduleTracker.GitInteraction.CLI;
public static class GitCLI
{ 
    public static async Task FetchAndPull(string path)
    {
        await GitProcessExecutor.ExecuteVoidCommand(path, "fetch");
        await GitProcessExecutor.ExecuteVoidCommand(path, "pull");
    }

    public static async Task Switch(string path, string branch)
    {
        string actualBranch = await GetCurrentBranch(path);

        // Already on the branch
        if (actualBranch == branch)
        {
            return;
        }

        await GitProcessExecutor.ExecuteVoidCommand(path, "submodule update --init --recursive");
        await GitProcessExecutor.ExecuteVoidCommand(path, $"switch {branch}");
    }

    [Obsolete("Switch je mejby lepsi")]
    public static async Task Checkout(string path, string branch)
    {
        await GitProcessExecutor.ExecuteVoidCommand(path, $"checkout {branch}");
        await GitProcessExecutor.ExecuteVoidCommand(path, "submodule update --init --recursive");
    }

    public static Task<string> GetCurrentBranch(string path)
    {
        // --show-current branch name is returned
        return GitProcessExecutor.ExecuteResponseCommand(path, "branch --show-current");
    }

    public static async Task<bool> AreChangesInWorkdir(string path)
    {
        // -s for short. Returns just modified files
        string modifiedFilesString = await GitProcessExecutor.ExecuteResponseCommand(path, "status -s");

        // no modified fiels
        return modifiedFilesString == string.Empty;
    }

    public static async Task StashChanges(string path)
    {
        await GitProcessExecutor.ExecuteVoidCommand(path, "stash");
    }

    public static async Task StashPop(string path)
    {
        await GitProcessExecutor.ExecuteVoidCommand(path, "stash pop");
    }
}