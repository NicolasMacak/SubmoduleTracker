using Microsoft.Extensions.DependencyInjection;
namespace SubmoduleTracker.Domain.Navigation;
public sealed class NavigationService
{
    private readonly IServiceProvider _serviceProvider;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void NavigateTo<TWorkflow>() where TWorkflow : class, IWorkflow // todo. Secure callings. They all need to return have return thing
    {
        Console.Clear();
        _serviceProvider.GetRequiredService<TWorkflow>().Run();
    }
}

public interface IWorkflow
{
    public void Run();
}