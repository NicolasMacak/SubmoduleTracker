namespace SubmoduleTracker.Core.GitInteraction.CommandExceptions;
/// <summary>
/// Thrown when command execution fails
/// </summary>
public sealed class CommandExecutionException : Exception
{
    public CommandExecutionException(string path, string command) 
        : base($"Command execution failed. \n Directory: {path} \n Command: {command}") { }
}