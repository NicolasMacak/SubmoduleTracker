using SubmoduleTracker.Core.CommonTypes.Menu;
using SubmoduleTracker.Core.CommonTypes.Result;
using SubmoduleTracker.Core.ConsoleTools;
using SubmoduleTracker.Core.ConsoleTools.Constants;
using SubmoduleTracker.Core.Navigation.Services;
using SubmoduleTracker.Domain.AlignmentExecution;
using SubmoduleTracker.Domain.AlignmentValidation;
using SubmoduleTracker.Domain.UserSettings;
using SubmoduleTracker.Domain.UserSettings.Services;

namespace SubmoduleTracker.Domain.HomeScreen;

public class HomeScreenWorkflow(NavigationService navigationService, UserConfigService userConfigFacade) : INavigable
{
    private readonly NavigationService _navigationService = navigationService;
    private readonly UserConfigService _userConfigFacade = userConfigFacade;

    public void Run()
    {
        CustomConsole.WriteLineColored("Main Menu", TextType.ImporantText);

        List<ActionMenuItem> homeScreenOptions = GetHomeScreenActions();

        if (_userConfigFacade.MetaSuperprojects.Count == 0)
        {
            CustomConsole.WriteLineColored("No superprojects added. In order to perform an action, add superprojects in the User Settings", TextType.Error);
        }

        int? choiceIndex = UserPrompts.GetIndexOfUserChoice(homeScreenOptions.Select(x => x.Title).ToList(), "Choose an action");
        if (!choiceIndex.HasValue)
        {
            throw new InvalidOperationException($"{nameof(ResultCode.EmptyInput)} is not valid in this scenario.");
        }

        homeScreenOptions[choiceIndex.Value].ItemAction();
    }

    private List<ActionMenuItem> GetHomeScreenActions()
    {
        List<ActionMenuItem> homeScreenActionsToReturn = [];

        if (_userConfigFacade.MetaSuperprojects.Count > 0)
        {
            homeScreenActionsToReturn.Add(new ActionMenuItem("Submodule Index Validation (Read-only)", _navigationService.NavigateTo<AlignmentValidationWorkflow>));
            homeScreenActionsToReturn.Add(new ActionMenuItem("Submodule Index Alignment Across Superprojects (Creates Commits, Switches branches)", _navigationService.NavigateTo<AlignmentExecutionWorkflow>));
        }

        homeScreenActionsToReturn.Add(new ActionMenuItem("User Settings", _navigationService.NavigateTo<ManageUserSettingsWorkflow>));

        return homeScreenActionsToReturn;
    }
}
