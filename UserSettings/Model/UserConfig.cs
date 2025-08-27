namespace SubmoduleTracker.UserSettings.Model;
public sealed class UserConfig
{
    public List<SuperProjectConfig> SuperProjects { get; set; } = new List<SuperProjectConfig>();
}

public record SuperProjectConfig(string WorkingDirectory);
