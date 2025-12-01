using SubmoduleTracker.Core.ConsoleTools;
using SubmoduleTracker.Core.MenuItems;
using SubmoduleTracker.Core.Result;
using SubmoduleTracker.Domain.AlignmentExecution;
using SubmoduleTracker.Domain.AlignmentValidation;
using SubmoduleTracker.Domain.Navigation;
using SubmoduleTracker.Domain.UserSettings;
using SubmoduleTracker.Domain.UserSettings.Services;

namespace SubmoduleTracker.Domain.HomeScreen;

public class HomeScreenWorkflow : IWorkflow
{
    private readonly NavigationService _navigationService;
    private readonly UserConfigService _userConfigFacade;

    public HomeScreenWorkflow(NavigationService navigationService, UserConfigService userConfigFacade)
    {
        _navigationService = navigationService;
        _userConfigFacade = userConfigFacade;
    }

    public void Run()
    {
        CustomConsole.WriteLineColored("Main Menu", TextType.ImporantText);

        List<MenuItem> homeScreenOptions = GetHomeScreenActions();

        if (_userConfigFacade.MetaSuperprojects.Count == 0)
        {
            CustomConsole.WriteLineColored("No superprojects added. In order to perform an action, add superprojects in the Settings", TextType.Error);
        }

        int? choiceIndex = CustomConsole.GetIndexOfUserChoice(homeScreenOptions.Select(x => x.Title).ToList(), "Choose an action");
        if (!choiceIndex.HasValue)
        {
            throw new InvalidOperationException($"{nameof(ResultCode.EmptyInput)} is not valid in this scenario.");
        }

        homeScreenOptions[choiceIndex.Value].ItemAction();
    }

    private List<MenuItem> GetHomeScreenActions()
    {
        List<MenuItem> homeScreenActionsToReturn = new();

        if (_userConfigFacade.MetaSuperprojects.Count > 0)
        {
            homeScreenActionsToReturn.Add(new MenuItem("Submodule Commit Index Validation (Read-only)", _navigationService.NavigateTo<AlignmentValidationWorkflow>));
            homeScreenActionsToReturn.Add(new MenuItem("Submodule Alignment Across Superprojects (Well. not Read-only)", _navigationService.NavigateTo<AlignmentExecutionWorkflow>));
        }

        homeScreenActionsToReturn.Add(new MenuItem("Settings", _navigationService.NavigateTo<ManageUserSettingsWorkflow>));

        return homeScreenActionsToReturn;
    }
}
