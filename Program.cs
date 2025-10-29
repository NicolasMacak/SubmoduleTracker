using LibGit2Sharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SubmoduleTracker.Domain.HomeScreen;
using SubmoduleTracker.Domain.SubmoduleAlignment;
using SubmoduleTracker.Domain.SubmoduleIndexValidation;
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

                // Home screen
                services.AddSingleton<ShowHomeScreenActionsWorkflow>();

                // Settings
                services.AddTransient<ManageUserSettingsWorkflow>();
                services.AddTransient<UserConfigFacade>();

                // AlignmentControl
                services.AddTransient<AlignmentValidationWorkflow>();

                // Aligning
                services.AddTransient<SubmoduleAlignmentWorkflow>();

                // Playing

                Repository repo = new("C:\\NON_SYSTEM\\Superproject-A");

                foreach (var branch in repo.Branches)
                {
                    Console.WriteLine($"Branch: {branch.FriendlyName}");

                    // Get the commit tree for the branch
                    var commit = branch.Tip;
                    if (commit == null) continue;

                    foreach (var entry in commit.Tree)
                    {
                        if (entry.TargetType == TreeEntryTargetType.GitLink)
                        {
                            Console.WriteLine($"  Submodule: {entry.Name} -> Commit {entry.Target.Id}");
                        }
                    }
                }
            });

        IHost app = host.Build();

        var aaa = app.Services.GetRequiredService<SubmoduleAlignmentWorkflow>();

        aaa.Run();

        return 0;
    }
}
