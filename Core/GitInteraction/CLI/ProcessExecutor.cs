using System.Diagnostics;
using SubmoduleTracker.Core.GitInteraction.CommandExceptions;

namespace SubmoduleTracker.Core.GitInteraction.CLI;
/// <summary>
/// Executes CLI commands at provided path
/// </summary>
public static class ProcessExecutor
{
    /// <summary>
    /// Creates and starts a git process for the specified command and working directory.
    /// </summary>
    /// <param name="path">The working directory where the git command will be executed.</param>
    /// <param name="command">The git command arguments to pass to the process.</param>
    private static Process? GetCommandProcess(string path, string command)
    {
        var psi = new ProcessStartInfo("git", command)
        {
            WorkingDirectory = path,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        return Process.Start(psi);
    }
    /// <summary>
    /// Executes a git command and returns the command's standard output as a single-line string.
    /// </summary>
    /// <param name="path">The working directory where the git command will be executed.</param>
    /// <param name="command">The git command arguments to execute.</param>
    /// <returns>The standard output of the executed command with line endings removed.</returns>
    public static string ExecuteResponseCommand(string path, string command)
    {
        Process? process = GetCommandProcess(path, command) ?? throw new InvalidOperationException("Failed to start git command process.");
        Console.WriteLine($"{path} >> {command}");

        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new CommandExecutionException(path, command);
        }

        return output.ReplaceLineEndings(string.Empty);
    }
    /// <summary>
    /// Executes a git command that does not require a return value.
    /// </summary>
    /// <param name="path">The working directory where the git command will be executed.</param>
    /// <param name="command">The git command arguments to execute.</param>
    /// <exception cref="InvalidOperationException">Thrown when the git process fails to start.</exception>
    /// <exception cref="CommandExecutionException">Thrown when the git command exits with a non-zero code.</exception>
    public static void ExecuteVoidCommand(string path, string command)
    {
        Process? process = GetCommandProcess(path, command) ?? throw new InvalidOperationException("Failed to start git command process.");
        Console.WriteLine($"{path} >> {command}");

        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new CommandExecutionException(path, command);
        }
    }
}