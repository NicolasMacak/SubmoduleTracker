using SubmoduleTracker.Core.CommonTypes.Menu;
using SubmoduleTracker.Core.CommonTypes.Result;
using SubmoduleTracker.Core.CommonTypes.SuperProjects;
using SubmoduleTracker.Core.ConsoleTools;
using SubmoduleTracker.Core.ConsoleTools.Constants;
using SubmoduleTracker.Core.Navigation.Services;
using SubmoduleTracker.Domain.HomeScreen;
using SubmoduleTracker.Domain.UserSettings.Services;

namespace SubmoduleTracker.Domain.UserSettings;

public class ManageUserSettingsWorkflow(UserConfigService userConfigFacade, NavigationService navigationService) : INavigable
{
    private readonly UserConfigService _userConfigFacade = userConfigFacade;
    private readonly NavigationService _navigationService = navigationService;

    public void Run()
    {
        Console.Clear();
        PrintUserConfig();

        List<ActionMenuItem> userSetttingsActions = GetUserSettingsActions();

        int? userSettingsMenuItemIndex = UserPrompts.GetIndexOfUserChoice(userSetttingsActions.Select(x => x.Title).ToList(), "Choose an action");
        if (!userSettingsMenuItemIndex.HasValue)
        {
            throw new InvalidOperationException($"{nameof(ResultCode.EmptyInput)} is not valid in this scenario.");
        }

        userSetttingsActions[userSettingsMenuItemIndex.Value].ItemAction();
    }

    private List<ActionMenuItem> GetUserSettingsActions()
    {
        List<ActionMenuItem> settingsActions = new()
        {
            new ActionMenuItem("Add Superproject", () => TryAddingNewSuperproject())
        };        

        if(_userConfigFacade.MetaSuperprojects.Count > 0)
        {
            settingsActions.Add(new ActionMenuItem("Remove Superproject", () => DeleteSuperproject()));
        }

        settingsActions.Add(new ActionMenuItem("Toggle \"Push to Remote\"", TogglePushingToRemote));
        settingsActions.Add(new ActionMenuItem("Back to Main Menu", _navigationService.NavigateTo<HomeScreenWorkflow>));

        return settingsActions;
    }

    /// <summary>
    /// Adds new superproject to userConfig. When error occurs. Called recurrsivelly until success or cancel by user
    /// </summary>
    /// <param name="userConfig"></param>
    /// <param name="errorMessage">Contains error message from last attempt</param>
    private void TryAddingNewSuperproject(string? errorMessage = null)
    {
        Console.Clear();

        // repeat until success
        while(true)
        {
            CustomConsole.WriteLineColored("Pridanie superprojektu", TextType.ImporantText);
            CustomConsole.WriteLineColored("Zadajte absolutnu cestu ku git repozitaru", TextType.Question);
            Console.WriteLine("Pre krok spat zadajte empty string" + Environment.NewLine);

            string? superprojectWorkdir = CustomConsole.ReadLine();

            // "" input. Back to user settings
            if (string.IsNullOrEmpty(superprojectWorkdir))
            {
                break;
            }

            // user input is handled here
            NonModelResult result = _userConfigFacade.AddSuperproject(superprojectWorkdir.Trim()!);

            if (result.ResultCode != ResultCode.Success)
            {
                Console.Clear();
                CustomConsole.WriteErrorLine(result.ErrorMessage!);
                continue;
            }

            break;
        }
        
        Run();
        return;
    }

    /// <summary>
    /// Remove superproject from config
    /// </summary>
    private void DeleteSuperproject(string? errorMessage = null)
    {
        Console.Clear();

        // repeat until success
        while (true)
        {
            List<string> superProjectsToDelete = _userConfigFacade.MetaSuperprojects.Select(x => x.WorkingDirectory).ToList();

            int? indexToDeleteIndex = UserPrompts.GetIndexOfUserChoice(superProjectsToDelete, Environment.NewLine + "Which superproject do you want to remove?", "Enter \"\" to end this action and return.");
            // Empty input => User wants step back
            if (!indexToDeleteIndex.HasValue)
            {
                Run();
                return;
            }

            NonModelResult result = _userConfigFacade.DeleteSuperProject(indexToDeleteIndex.Value);

            if (result.ResultCode != ResultCode.Success)
            {
                Console.Clear();
                CustomConsole.WriteErrorLine(result.ErrorMessage!);
                continue;
            }

            break;
        }

        Run();
        return;
    }

    private void TogglePushingToRemote()
    {
        NonModelResult result = _userConfigFacade.TogglePushingToRemote();

        if (result.ResultCode != ResultCode.Success)
        {
            CustomConsole.WriteErrorLine(result.ErrorMessage!);
        }

        Run();
        return;
    }

    private void PrintUserConfig()
    {
        CustomConsole.WriteLineColored("User Settings", TextType.ImporantText);
        CustomConsole.WriteLineColored("Superprojects: " + Environment.NewLine + "[", TextType.MundaneText);

        foreach(MetaSuperProject superproject in _userConfigFacade.MetaSuperprojects)
        {
            // We separate path by folders
            // last element is the name of the superproject(Which we want to highlight)
            string[] pathParts = superproject.WorkingDirectory.Split(@"\"); 

            // We deconstruct the path to separate if to parts that we want highlithed and parts that we dont
            IEnumerable<string> unghighlitherParts = pathParts.Take(pathParts.Length - 1);
            string unghighlightedString = string.Join(@"/", unghighlitherParts); 
            Console.Write("    " + unghighlightedString + @"/");

            // Superproject name is highlithed
            string superProjectName = pathParts[pathParts.Length - 1];
            CustomConsole.WriteLineColored(superProjectName, TextType.ImporantText);
        }
        CustomConsole.WriteLineColored("]", TextType.MundaneText);

        CustomConsole.WriteColored("Push to remote: ", TextType.MundaneText);
        if (_userConfigFacade.PushingToRemote)
        {
            CustomConsole.WriteColored(_userConfigFacade.PushingToRemote.ToString(), ConsoleColor.DarkGreen);
            CustomConsole.WriteColored(" // Application is allowed to execute `git push`, but will ask for permission", TextType.MundaneText);
        }
        else
        {
            CustomConsole.WriteColored(_userConfigFacade.PushingToRemote.ToString(), ConsoleColor.DarkGray);
            CustomConsole.WriteColored(" // `git push` is prohibited for the application", TextType.MundaneText);
        }

        CustomConsole.WriteLineColored(Environment.NewLine + Environment.NewLine + "Data saved at: C:\\Users\\{user}\\AppData\\Roaming", TextType.MundaneText);
        Console.WriteLine();
    }
}