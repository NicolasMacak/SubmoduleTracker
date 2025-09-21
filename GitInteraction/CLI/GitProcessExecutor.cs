﻿using System.Diagnostics;
using SubmoduleTracker.GitInteraction.CommandExceptions;

namespace SubmoduleTracker.GitInteraction.CLI;
public static class GitProcessExecutor
{
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

        if (process.ExitCode != 0)
        {
            throw new CommandExecutionException(path, command);
        }

        return output.ReplaceLineEndings(string.Empty);
    }

    public static void ExecuteVoidCommand(string path, string command)
    {
        Process? process = GetCommandProcess(path, command);
        if (process == null)
        {
            throw new InvalidOperationException("Failed to start git command process.");
        }

        Console.WriteLine($"{path} >> {command}");

        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new CommandExecutionException(path, command);
        }

    }
}