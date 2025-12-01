using SubmoduleTracker.Core.GitInteraction.Model;

namespace SubmoduleTracker.Domain.UserSettings.Model;
public sealed class UserConfig
{
    public List<string> SuperProjectsWorkdirs { get; set; } = new ();

    public bool PushingToRemote { get; set; } = false;

    public bool ContainsSuperproject(string workingDirectory)
    {
        // We exclude '\' from comparison There would be different count of '\' for path in newly added superproject and path added by user would be inconsisent. 
        // different amount of '\' can be contained in the name based on user input. But functionally, it's not issue. Program to aj tak zje
        return SuperProjectsWorkdirs.Any(x => x.Replace(@"\", string.Empty) == workingDirectory!.Replace(@"\", string.Empty));
    }
}

