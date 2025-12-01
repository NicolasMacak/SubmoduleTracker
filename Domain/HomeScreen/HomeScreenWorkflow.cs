using SubmoduleTracker.Core.ConsoleTools;
using SubmoduleTracker.Core.Result;
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
        CustomConsole.WriteLineColored("Main Menu", TextType.ImporantText);

        List<(string menuItem, Action navigationAction)> homeScreenOptions = GetHomeScreenActions();

        if (_userConfigFacade.MetaSupeprojects.Count == 0)
        {
            CustomConsole.WriteLineColored("No superprojects added. In order to perform an action, add superprojects in the Settings", TextType.Error);
        }

        ModelResult<int> choiceIndex = CustomConsole.GetIndexOfUserChoice(homeScreenOptions.Select(x => x.menuItem).ToList(), "Choose an action");
        if (choiceIndex.ResultCode == ResultCode.EmptyInput)
        {
            throw new InvalidOperationException($"{nameof(ResultCode.EmptyInput)} is not valid in this scenario.");
        }

        homeScreenOptions[choiceIndex.Model].navigationAction();
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
