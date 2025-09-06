using System.Diagnostics;
using SubmoduleTracker.ConsoleTools;

namespace SubmoduleTracker.GitInteraction.CLI;
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

        try
        {
            await process.WaitForExitAsync();
            return output.ReplaceLineEndings(string.Empty);
        }
        catch (Exception ex)
        {
            string error = await process.StandardError.ReadToEndAsync();
            PrintError(path, command, error, ex.Message);
            throw;
            //string error = await process.StandardError.ReadToEndAsync();
            //if (!string.IsNullOrEmpty(error))
            //{
            //    PrintError(path, command, error);
            //}
        }
    }

    public static async Task ExecuteVoidCommand(string path, string command)
    {
        Process? process = GetCommandProcess(path, command);
        if (process == null)
        {
            throw new InvalidOperationException("Failed to start git command process.");
        }

        try
        {
            await process.WaitForExitAsync();
        }
        catch (Exception ex)
        {
            string error = await process.StandardError.ReadToEndAsync();
            PrintError(path, command, error, ex.Message);
            throw;
        }
    }

    private static void PrintError(string path, string command, string error, string exception)
    {
        CustomConsole.WriteErrorLine($"Directory: {path}");
        CustomConsole.WriteErrorLine($"Command: {command}");
        CustomConsole.WriteErrorLine($"Error message: {error}");
        CustomConsole.WriteErrorLine($"Exception: {exception}");
    }
}