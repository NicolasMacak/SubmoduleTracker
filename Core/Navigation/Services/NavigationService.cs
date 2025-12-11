using Microsoft.Extensions.DependencyInjection;
using SubmoduleTracker.Core.CommonTypes.Menu;
using SubmoduleTracker.Core.ConsoleTools;
using SubmoduleTracker.Domain.HomeScreen;
namespace SubmoduleTracker.Core.Navigation.Services;
/// <summary>
/// Service that allows to change screens
/// </summary>
public sealed class NavigationService(IServiceProvider serviceProvider)
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    /// <summary>
    /// Navigates to the provided workflow
    /// </summary>
    /// <typeparam name="TNavigable">Workflow to navigate to</typeparam>
    public void NavigateTo<TNavigable>() where TNavigable : class, INavigable
    {
        Console.Clear();
        _serviceProvider.GetRequiredService<TNavigable>().Run();
    }

    public void PromptReturnToMainMenu()
    {
        List<ActionMenuItem> actions = new()
        {
            new("Return to main menu", NavigateTo<HomeScreenWorkflow>),
            new("Exit Application", () => { return; })
        };

        int? choice = UserPrompts.GetIndexOfUserChoice(actions.Select(x => x.Title).ToList(), "Next step?");
        if (!choice.HasValue)
        {
            return;
        }

        actions[choice.Value].ItemAction();
    }
}
/// <summary>
/// Classes implementing this interface are screens/workflows that users can navigate to
/// </summary>
public interface INavigable
{
    public void Run();
}