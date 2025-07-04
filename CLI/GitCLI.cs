namespace SubmoduleTracker.CLI;
public static class GitCLI
{
    public static async Task Checkout(string path, string branch)
    {
        await GitProcessExecutor.ExecuteVoidCommand(path, $"checkout {branch}");
        await GitProcessExecutor.ExecuteVoidCommand(path, "submodule update --init --recursive");
    }

    public static Task<string> GetCurrentBranch(string path)
    {
        return GitProcessExecutor.ExecuteResponseCommand(path, "branch --show-current");
    }
}