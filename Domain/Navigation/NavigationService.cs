using Microsoft.Extensions.DependencyInjection;
namespace SubmoduleTracker.Domain.Navigation;
public sealed class NavigationService
{
    private readonly IServiceProvider _serviceProvider;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void Navigate(Type workflowToNavigateTo)
    {
        if (!typeof(IWorkflow).IsAssignableFrom(workflowToNavigateTo))
        {
            throw new InvalidCastException($"Class to navigate must implement {nameof(IWorkflow)} interface!");
        }

        IWorkflow worfkflow = (IWorkflow)_serviceProvider.GetRequiredService(workflowToNavigateTo);

        worfkflow.Run();
    }
}

public interface IWorkflow
{
    public void Run();
}