using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SubmoduleTracker.Domain.HomeScreen;
using SubmoduleTracker.Domain.AlignmentExecution;
using SubmoduleTracker.Domain.AlignmentValidation;
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
                services.AddTransient<AlignmentExecutionWorkflow>();

                // Playing

                //Repository repo = new("C:\\NON_SYSTEM\\Superproject-A");

                //foreach (var branch in repo.Branches)
                //{
                //    Console.WriteLine($"Branch: {branch.FriendlyName}");

                //    // Get the commit tree for the branch
                //    var commit = branch.Tip;
                //    if (commit == null) continue;

                //    foreach (var entry in commit.Tree)
                //    {
                //        if (entry.TargetType == TreeEntryTargetType.GitLink)
                //        {
                //            Console.WriteLine($"  Submodule: {entry.Name} -> Commit {entry.Target.Id}");
                //        }
                //    }
                //}



                //git fetch --all--recurse - submodules
                //git submodule update--init--recursive
            });

        IHost app = host.Build();

        var aaa = app.Services.GetRequiredService<AlignmentExecutionWorkflow>();

        aaa.Run();

        return 0;
    }
}
