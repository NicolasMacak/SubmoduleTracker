namespace SubmoduleTracker.Core.CommonTypes.Menu;
/// <summary>
/// Item in dynamically created menu
/// </summary>
public class ActionMenuItem
{
    /// <summary>
    /// Title of the action
    /// </summary>
    public string Title;
    /// <summary>
    /// Action to be triggered
    /// </summary>
    public Action ItemAction;

    public ActionMenuItem(string title, Action itemAction)
    {
        Title = title;
        ItemAction = itemAction;
    }
}

