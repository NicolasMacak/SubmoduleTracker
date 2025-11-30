using SubmoduleTracker.Core.ConsoleTools;
using SubmoduleTracker.Domain.AlignmentValidation;
using SubmoduleTracker.Domain.Navigation;
using SubmoduleTracker.Domain.UserSettings;
using SubmoduleTracker.Domain.UserSettings.Services;

namespace SubmoduleTracker.Domain.HomeScreen;

public class HomeScreenWorkflow : IWorkflow
{
    private readonly NavigationService _navigationService;
    private readonly UserConfigFacade _userConfigFacade;

    public HomeScreenWorkflow(NavigationService navigationService, UserConfigFacade userConfigFacade)
    {
        _navigationService = navigationService;
        _userConfigFacade = userConfigFacade;
    }

    public void Run()
    {
        Console.Clear();
        CustomConsole.WriteLineColored("Main Menu", TextType.ImporantText);

        List<(string menuItem, Action navigationAction)> homeScreenOptions = GetHomeScreenActions();

        if (_userConfigFacade.MetaSupeprojects.Count == 0)
        {
            CustomConsole.WriteLineColored("No superprojects added. In order to perform an action, add superprojects in the Settings", TextType.Error);
        }

        int? choice = CustomConsole.GetIndexOfUserChoice(homeScreenOptions.Select(x => x.menuItem).ToList(), "Choose an action");

        if (!choice.HasValue)
        {
            throw new ArgumentNullException("User choice is not supposed to be null");
        }

        homeScreenOptions[choice.Value].navigationAction();
    }

    private List<(string menuItem, Action navigationAction)> GetHomeScreenActions()
    {
        List<(string menuItem, Action navigationAction)> homeScreenActionsToReturn = new();

        if (_userConfigFacade.MetaSupeprojects.Count > 0)
        {
            homeScreenActionsToReturn.Add(new("Submodule Commit Index Validation (Read-only)", _navigationService.NavigateTo<AlignmentValidationWorkflow>));
            homeScreenActionsToReturn.Add(new("Submodule Alignment Across Superprojects (Well. not Read-only)", _navigationService.NavigateTo<AlignmentValidationWorkflow>));
        }

        homeScreenActionsToReturn.Add(new("Settings", _navigationService.NavigateTo<ManageUserSettingsWorkflow>));

        return homeScreenActionsToReturn;
    }
}
