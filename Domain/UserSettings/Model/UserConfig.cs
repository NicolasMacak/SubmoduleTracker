namespace SubmoduleTracker.Domain.UserSettings.Model;
public sealed class UserConfig
{
    public List<SuperProjectConfig> SuperProjects { get; set; } = new ();

    public bool PushingToRemote => false;

    public bool ContainsSuperproject(string workingDirectory)
    {
        // We exclude '\' from comparison There would be different count of '\' for path in newly added superproject and path added by user would be inconsisent. 
        // different amount of '\' can be contained in the name based on user input. But functionally, it's not issue. Program to aj tak zje
        return SuperProjects.Any(x => x.WorkingDirectory.Replace(@"\", string.Empty) == workingDirectory!.Replace(@"\", string.Empty));
    }
}


public record SuperProjectConfig(string WorkingDirectory);
