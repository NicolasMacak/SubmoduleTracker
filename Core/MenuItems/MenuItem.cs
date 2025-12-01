using SubmoduleTracker.Core.Result;

namespace SubmoduleTracker.Core.MenuItems;
public class MenuItem
{
    public string Title;
    public Action ItemAction;

    public MenuItem(string title, Action itemAction)
    {
        Title = title;
        ItemAction = itemAction;
    }
}

