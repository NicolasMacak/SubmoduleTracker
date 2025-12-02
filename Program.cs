using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SubmoduleTracker.Domain.AlignmentExecution;
using SubmoduleTracker.Domain.AlignmentValidation;
using SubmoduleTracker.Domain.UserSettings;
using SubmoduleTracker.Domain.UserSettings.Services;
using SubmoduleTracker.Domain.HomeScreen;
using SubmoduleTracker.Core.Navigation.Services;

class Program
{
    public static int Main(string[] args)
    {
        IHostBuilder host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // Navigation
                services.AddTransient<NavigationService>();

                // Home screen
                services.AddTransient<HomeScreenWorkflow>();

                // User Settigns
                services.AddTransient<ManageUserSettingsWorkflow>();
                services.AddTransient<UserConfigService>();

                // Alignment Validation
                services.AddTransient<AlignmentValidationWorkflow>();

                // Alignment Execution
                services.AddTransient<AlignmentExecutionWorkflow>();
            });

        IHost app = host.Build();

        app.Services.GetRequiredService<HomeScreenWorkflow>().Run();

        return 0;
    }
}
