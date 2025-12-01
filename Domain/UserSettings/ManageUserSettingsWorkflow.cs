using SubmoduleTracker.Core.ConsoleTools;
using SubmoduleTracker.Core.GitInteraction.Model;
using SubmoduleTracker.Core.MenuItems;
using SubmoduleTracker.Core.Result;
using SubmoduleTracker.Domain.HomeScreen;
using SubmoduleTracker.Domain.Navigation;
using SubmoduleTracker.Domain.UserSettings.Services;

namespace SubmoduleTracker.Domain.UserSettings;

public class ManageUserSettingsWorkflow : IWorkflow
{
    private readonly UserConfigFacade _userConfigFacade;
    private readonly NavigationService _navigationService;

    public ManageUserSettingsWorkflow(UserConfigFacade userConfigFacade, NavigationService navigationService)
    {
        _userConfigFacade = userConfigFacade;
        _navigationService = navigationService;
    }

    public void Run()
    {
        Console.Clear();
        PrintUserConfig();

        List<MenuItem> userSetttingsActions = GetUserSettingsActions();

        int? userSettingsMenuItemIndex = CustomConsole.GetIndexOfUserChoice(userSetttingsActions.Select(x => x.Title).ToList(), "Choose an action");
        if (!userSettingsMenuItemIndex.HasValue)
        {
            throw new InvalidOperationException($"{nameof(ResultCode.EmptyInput)} is not valid in this scenario.");
        }

        userSetttingsActions[userSettingsMenuItemIndex.Value].ItemAction();
    }

    private List<MenuItem> GetUserSettingsActions()
    {
        List<MenuItem> settingsActions = new()
        {
            new MenuItem("Add Superproject", () => TryAddingNewSuperproject())
        };        

        if(_userConfigFacade.ConfigSuperProjects.Count > 0)
        {
            settingsActions.Add(new MenuItem("Remove Superproject", () => DeleteSuperproject()));
        }

        settingsActions.Add(new MenuItem("Toggle \"Push to Remote\"", TogglePushingToRemote));
        settingsActions.Add(new MenuItem("Back to Main Menu", _navigationService.NavigateTo<HomeScreenWorkflow>));

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

            string? superprojectWorkdir = Console.ReadLine();

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
            List<string> superProjectsToDelete = _userConfigFacade.ConfigSuperProjects.Select(x => x.WorkingDirectory).ToList();

            int? indexToDeleteIndex = CustomConsole.GetIndexOfUserChoice(superProjectsToDelete, Environment.NewLine + "Ktory superprojekt chcete zmazat?", "Alebo \"\" pre ukoncenie tejto akcie");
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
        CustomConsole.WriteLineColored("Configuration settigns", TextType.ImporantText);
        CustomConsole.WriteLineColored("Superprojects: " + Environment.NewLine + "[", TextType.MundaneText);

        foreach(ConfigSuperProject superproject in _userConfigFacade.ConfigSuperProjects)
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
        CustomConsole.WriteColored(_userConfigFacade.PushingToRemote.ToString(), _userConfigFacade.PushingToRemote ? ConsoleColor.DarkGreen : ConsoleColor.DarkGray);
        CustomConsole.WriteColored(" // Ak False, aplikacia ma zakazane pushovat na remote", TextType.MundaneText);

        CustomConsole.WriteLineColored(Environment.NewLine + Environment.NewLine + "Data saved at: C:\\Users\\{user}\\AppData\\Roaming", TextType.MundaneText);
        Console.WriteLine();
    }
}