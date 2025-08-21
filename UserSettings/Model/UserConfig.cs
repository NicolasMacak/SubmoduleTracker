namespace SubmoduleTracker.UserSettings.Model;
public sealed class UserConfig
{
    public List<SuperProjectConfig> SuperProjects { get; set; }

    public List<SubmoduleConfig> Submodules { get; set; }
}

public record SuperProjectConfig(string WorkingDirectory);

public record SubmoduleConfig(string WorkingDirectory);