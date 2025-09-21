namespace SubmoduleTracker.Core.GitInteraction.Model;
public sealed class ConfigSuperProject
{
    public readonly string WorkingDirectory;

    public ConfigSuperProject(string workingDirectory)
    {
        WorkingDirectory = workingDirectory;
    }
}