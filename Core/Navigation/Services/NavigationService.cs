using Microsoft.Extensions.DependencyInjection;
namespace SubmoduleTracker.Core.Navigation.Services;
/// <summary>
/// Service that allows to change screens
/// </summary>
public sealed class NavigationService
{
    private readonly IServiceProvider _serviceProvider;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Navigates to the provided workflow
    /// </summary>
    /// <typeparam name="TNavigable">Workflow to navigate to</typeparam>
    public void NavigateTo<TNavigable>() where TNavigable : class, INavigable
    {
        Console.Clear();
        _serviceProvider.GetRequiredService<TNavigable>().Run();
    }
}
/// <summary>
/// Classes implementing this interface are screens/workflows that users can navigate to
/// </summary>
public interface INavigable
{
    public void Run();
}