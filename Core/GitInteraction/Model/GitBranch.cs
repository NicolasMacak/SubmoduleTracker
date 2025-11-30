namespace SubmoduleTracker.Core.GitInteraction.Model;
public class GitBranch(string localName)
{
    public readonly string LocalName = localName;
    public readonly string RemoteName = $"origin/{localName}";
}

