namespace SubmoduleTracker.GitInteraction.CommandExceptions;
public sealed class CommandExecutionException : Exception
{
    public CommandExecutionException(string path, string command) 
        : base($"Command execution failed. \n Directory: {path} \n Command: {command}") { }
}