// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SubmoduleTracker.Domain.SubmoduleAlignment;
using SubmoduleTracker.Domain.UserSettings;
using SubmoduleTracker.Domain.UserSettings.Services;


class Program
{
    public static int Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<UserConfigFacade>();

                services.AddTransient<ManageUserSettingsWorkflow>();
                services.AddTransient<AlignSubmodulesWorkflow>();
            });

        return 0;
    }
}

//var aaa = GitFacade.GetCurrentBranch("C:\\NON_SYSTEM");
//GitFacade.Switch("C:\\NON_SYSTEM\\Submodule-C", "dev");

//SubmoduleTracker.UserSettings.Model.UserConfig userConfig = UserSettingsScreen.GetUserConfiguration();

//SubmoduleAlignment.Index(userConfig);

//return 0;
