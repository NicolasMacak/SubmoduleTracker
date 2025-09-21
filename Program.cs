using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SubmoduleTracker.Domain.HomeScreen;
using SubmoduleTracker.Domain.SubmoduleAlignment;
using SubmoduleTracker.Domain.UserSettings;
using SubmoduleTracker.Domain.UserSettings.Services;

class Program
{
    public static int Main(string[] args)
    {
        IHostBuilder host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<UserConfigFacade>();

                services.AddSingleton<ShowHomeScreenActionsWorkflow>();

                services.AddTransient<ManageUserSettingsWorkflow>();
                services.AddTransient<UserConfigFacade>();

                services.AddTransient<AlignSubmodulesWorkflow>();
            });

        IHost app = host.Build();

        var aaa = app.Services.GetRequiredService<ManageUserSettingsWorkflow>();

        aaa.Run();

        return 0;
    }
}
