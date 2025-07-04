using System.Diagnostics;

namespace SubmoduleTracker.CLI;
public static class GitProcessExecutor
{
    private static Process? GetCommandProcess(string path, string command)
    {
        var psi = new ProcessStartInfo("git", command)
        //var psi = new ProcessStartInfo("git", "fetch --all")
        {
            WorkingDirectory = path,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        return Process.Start(psi);
    }

    public static async Task<string> ExecuteResponseCommand(string path, string command)
    {
        Process? process = GetCommandProcess(path, command);
        if (process == null)
        {
            throw new InvalidOperationException("Failed to start git command process.");
        }

        string output = await process.StandardOutput.ReadToEndAsync();
        string error = await process.StandardError.ReadToEndAsync(); // todo osetrit
        process.WaitForExit();

        return output.ReplaceLineEndings(string.Empty);
    }

    public static async Task ExecuteVoidCommand(string path, string command)
    {
        Process? process = GetCommandProcess(path, command);
        if (process == null)
        {
            throw new InvalidOperationException("Failed to start git command process.");
        }

        string error = await process.StandardError.ReadToEndAsync();
        process.WaitForExit();
    }
}