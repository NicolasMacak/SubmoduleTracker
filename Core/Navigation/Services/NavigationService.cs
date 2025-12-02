using Microsoft.Extensions.DependencyInjection;
namespace SubmoduleTracker.Core.Navigation.Services;
public sealed class NavigationService
{
    private readonly IServiceProvider _serviceProvider;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void NavigateTo<TWorkflow>() where TWorkflow : class, IWorkflow
    {
        Console.Clear();
        _serviceProvider.GetRequiredService<TWorkflow>().Run();
    }
}

public interface IWorkflow
{
    public void Run();
}