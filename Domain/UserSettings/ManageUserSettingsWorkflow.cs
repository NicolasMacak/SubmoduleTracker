using SubmoduleTracker.Core.ConsoleTools;
using SubmoduleTracker.Core.GitInteraction.Model;
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

        List<(string menuItem, Action userSettingsAction)> userSetttingsActions = GetUserSettingsActions();

        int? validatedChoice = CustomConsole.GetIndexOfUserChoice(userSetttingsActions.Select(x => x.menuItem).ToList(), "Zvolte akciu");
        if (!validatedChoice.HasValue)
        {
            throw new Exception("User choice is not supposed to be null");
        }

        userSetttingsActions[validatedChoice.Value].userSettingsAction();
    }

    private List<(string menuItem, Action userSettingsAction)> GetUserSettingsActions()
    {
        List<(string menuItem, Action userSettingsAction)> settingsActions = new();

        settingsActions.Add(("Add Superproject", () => TryAddingNewSuperproject()));

        if(_userConfigFacade.MetaSupeprojects.Count > 0)
        {
            settingsActions.Add(("Remove Superproject", () => DeleteSuperproject()));
        }

        settingsActions.Add(("Toggle \"Push to Remote\"", () => TogglePushingToRemote()));
        settingsActions.Add(("Back to Main Menu", _navigationService.NavigateTo<HomeScreenWorkflow>));

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
        if (!string.IsNullOrEmpty(errorMessage)) {
            CustomConsole.WriteErrorLine(errorMessage + Environment.NewLine);
        }

        CustomConsole.WriteLineColored("Pridanie superprojektu", TextType.ImporantText);
        CustomConsole.WriteLineColored("Zadajte absolutnu cestu ku git repozitaru", TextType.Question);
        Console.WriteLine("Pre krok spat zadajte empty string" + Environment.NewLine);

        string? superprojectWorkdir = Console.ReadLine();

        // "" input. Cancel action 
        if (string.IsNullOrEmpty(superprojectWorkdir))
        {
            Run();
        }

        // user input is handled here
        NonModelResult result = _userConfigFacade.AddSuperproject(superprojectWorkdir!);

        if (result.ResultCode != ResultCode.Success)
        {
            TryAddingNewSuperproject(result.ErrorMessage);
        }

        Run();
    }

    /// <summary>
    /// Remove superproject from config
    /// </summary>
    private void DeleteSuperproject(string? errorMessage = null)
    {
        if (!string.IsNullOrEmpty(errorMessage))
        {
            CustomConsole.WriteErrorLine(errorMessage + Environment.NewLine);
        }

        List<string> superProjectsToDelete = _userConfigFacade.ConfigSuperProjects.Select(x => x.WorkingDirectory).ToList();

        int? indexToDeleteAt = CustomConsole.GetIndexOfUserChoice(superProjectsToDelete, Environment.NewLine + "Ktory superprojekt chcete zmazat?", "Alebo \"\" pre ukoncenie tejto akcie");

        // "" input. Cancel action
        if (!indexToDeleteAt.HasValue)
        {
            Run();
        }

        NonModelResult result = _userConfigFacade.DeleteSuperProject(indexToDeleteAt!.Value);

        if (result.ResultCode != ResultCode.Success)
        {
            DeleteSuperproject(result.ErrorMessage); // todo. Refactoring. Malo by ma to vratit na hlavnu obrazovku?
        }

        Run();
    }

    private void TogglePushingToRemote()
    {
        NonModelResult result = _userConfigFacade.TogglePushingToRemote();

        if (result.ResultCode != ResultCode.Success)
        {
            CustomConsole.WriteErrorLine(result.ErrorMessage!);
        }

        Run();
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