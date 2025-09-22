﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SubmoduleTracker.Domain.HomeScreen;
using SubmoduleTracker.Domain.SubmoduleAlignment;
using SubmoduleTracker.Domain.SubmoduleIndexValidation;
using SubmoduleTracker.Domain.UserSettings;
using SubmoduleTracker.Domain.UserSettings.Services;

/*
 */
class Program
{
    public static int Main(string[] args)
    {
        IHostBuilder host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<UserConfigFacade>();

                // Home screen
                services.AddSingleton<ShowHomeScreenActionsWorkflow>();

                // Settings
                services.AddTransient<ManageUserSettingsWorkflow>();
                services.AddTransient<UserConfigFacade>();

                // AlignmentControl
                services.AddTransient<AlignmentValidationWorkflow>();

                // Aligning
                services.AddTransient<SubmoduleAlignmentWorkflow>();
            });

        IHost app = host.Build();

        var aaa = app.Services.GetRequiredService<AlignmentValidationWorkflow>();

        aaa.Run();

        return 0;
    }
}
