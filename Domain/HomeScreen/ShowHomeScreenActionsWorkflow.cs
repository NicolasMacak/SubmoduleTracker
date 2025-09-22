using SubmoduleTracker.Core.GitInteraction.Model;
using SubmoduleTracker.Domain.SubmoduleIndexValidation;
using SubmoduleTracker.Domain.SubmoduleIndexValidation.Dto;
using SubmoduleTracker.Domain.UserSettings.Model;
using SubmoduleTracker.Domain.UserSettings.Services;

namespace SubmoduleTracker.Domain.HomeScreen;


/*
 
Nacitanie superprojektu nacita submoduly

Submoduly budu taktiez evidovane

listing to bude urco. a mejby farby.

co krok, to save

*/
public class ShowHomeScreenActionsWorkflow
{
    private readonly UserConfigFacade _userConfigfacade;

    public ShowHomeScreenActionsWorkflow(UserConfigFacade  userConfig)
    {
        _userConfigfacade = userConfig;
    }

    public void ShowHomeScreen()
    {
        // get config file

        // Superprojects
        //ManageUserSettingsWorkflow.Run();

        // Submodules
    }

    public static void ShowMainOptions()
    {
        Console.WriteLine("1. Zmenit nastavenia");
        Console.WriteLine("2. Porovnat submoduly");
    }

    public void GenerateReport(string superProjectPath)
    {
    }
}
