using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SubmoduleTracker.Domain.Navigation;
using SubmoduleTracker.Domain.AlignmentExecution;
using SubmoduleTracker.Domain.AlignmentValidation;
using SubmoduleTracker.Domain.UserSettings;
using SubmoduleTracker.Domain.UserSettings.Services;
using SubmoduleTracker.Domain.HomeScreen;

class Program
{
    public static int Main(string[] args)
    {
        IHostBuilder host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<UserConfigFacade>();

                // Navigation
                services.AddTransient<HomeScreenWorkflow>();
                services.AddTransient<NavigationService>();

                // Settings
                services.AddTransient<ManageUserSettingsWorkflow>();
                services.AddTransient<UserConfigFacade>();

                // AlignmentControl
                services.AddTransient<AlignmentValidationWorkflow>();

                // Aligning
                services.AddTransient<AlignmentExecutionWorkflow>();
            });

        IHost app = host.Build();

        app.Services.GetRequiredService<HomeScreenWorkflow>().Run();

        return 0;
    }
}
