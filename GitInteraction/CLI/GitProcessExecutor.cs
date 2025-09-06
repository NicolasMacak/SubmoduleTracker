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

    public static string ExecuteResponseCommand(string path, string command)
    {
        Process? process = GetCommandProcess(path, command);
        if (process == null)
        {
            throw new InvalidOperationException("Failed to start git command process.");
        }

        Console.WriteLine($"{path} >> {command}");

        //process.Start();
        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        return output.ReplaceLineEndings(string.Empty);

        //try
        //{
        //    process.WaitForExitAsync();
        //    return output.ReplaceLineEndings(string.Empty);
        //}
        //catch (Exception ex)
        //{
        //    string error = process.StandardError.ReadToEndAsync();
        //    PrintError(path, command, error, ex.Message);
        //    throw;
        //}
    }

    public static void ExecuteVoidCommand(string path, string command)
    {
        Process? process = GetCommandProcess(path, command);
        if (process == null)
        {
            throw new InvalidOperationException("Failed to start git command process.");
        }

        Console.WriteLine($"{path} >> {command}");

        //process.Start();
        //string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        //if (process.ExitCode != 0)
        //{
        //    string error = process.StandardError.ReadToEnd();
        //    PrintError(path, command, error, $"Exit code: {process.ExitCode}");
        //    throw new InvalidOperationException($"Git command failed: {error}");
        //}
    }
        //try
        //{
        //    process.WaitForExitAsync();
        //}
        //catch (Exception ex)
        //{
        //    string error = process.StandardError.ReadToEndAsync();
        //    PrintError(path, command, error, ex.Message);
        //    throw;
        //}

    private static void PrintError(string path, string command, string error, string exception)
    {
        CustomConsole.WriteErrorLine($"Directory: {path}");
        CustomConsole.WriteErrorLine($"Command: {command}");
        CustomConsole.WriteErrorLine($"Error message: {error}");
        CustomConsole.WriteErrorLine($"Exception: {exception}");
    }
}