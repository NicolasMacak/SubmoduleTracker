using SubmoduleTracker.Core.ConsoleTools;
using SubmoduleTracker.Core.GitInteraction.Model;
using SubmoduleTracker.Core.Result;
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

    public void Run(/*string? errorMessage = null*/)
    {
        Console.Clear();
        //if (!string.IsNullOrEmpty(errorMessage))
        //{
        //    CustomConsole.WriteErrorLine(errorMessage + Environment.NewLine);
        //}

        PrintUserConfig();

        List<string> menuOptions = 
        [
            "Pridat superprojekt",
            "Zmazat superprojekt",
            "Toggle pushovanie na remote // Ak False, aplikacia ma zakazane pushovat na remote",
            "Spat do hlavneho menu"
        ];

        int? validatedChoice = CustomConsole.GetIndexFromChoices(menuOptions, "Zvolte akciu");

        if (!validatedChoice.HasValue)
        {
            //Console.Clear();
            //Run($"Invalid input. Must be number from 0 to {menuOptionsCount - 1}");
        }

        switch (validatedChoice!.Value) {
            case 0:
                // Add Superproject to the userConfig and save
                TryAddingNewSuperproject();
                break;

            case 1: DeleteSuperproject(); break;
            case 2: TogglePushingToRemote(); break;
            case 3: _navigationService.Navigate(typeof(HomeScreenWorkflow)); break;
            default: Console.WriteLine("Taka moznost nie je!"); break;
        }
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
        VoidResult result = _userConfigFacade.AddSuperproject(superprojectWorkdir!);

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

        int? indexToDeleteAt = CustomConsole.GetIndexFromChoices(superProjectsToDelete, Environment.NewLine + "Ktory superprojekt chcete zmazat?", "Alebo \"\" pre ukoncenie tejto akcie");

        // "" input. Cancel action
        if (!indexToDeleteAt.HasValue)
        {
            Run();
        }

        VoidResult result = _userConfigFacade.DeleteSuperProject(indexToDeleteAt!.Value);

        if (result.ResultCode != ResultCode.Success)
        {
            DeleteSuperproject(result.ErrorMessage); // todo. Refactoring. Malo by ma to vratit na hlavnu obrazovku?
        }

        Console.Clear();
        Run();
    }

    private void TogglePushingToRemote()
    {
        VoidResult result = _userConfigFacade.TogglePushingToRemote();

        if (result.ResultCode != ResultCode.Success)
        {
            CustomConsole.WriteErrorLine(result.ErrorMessage);
        }

        Run();
    }

    private void PrintUserConfig()
    {
        CustomConsole.WriteLineColored("Configuration settigns", TextType.ImporantText);
        CustomConsole.WriteLineColored("Superprojects: ", TextType.MundaneText);

        foreach(ConfigSuperProject superproject in _userConfigFacade.ConfigSuperProjects)
        {
            // We separate path by folders
            // last element is the name of the superproject(Which we want to highlight)
            string[] pathParts = superproject.WorkingDirectory.Split(@"\"); 

            // We deconstruct the path to separate if to parts that we want highlithed and parts that we dont
            IEnumerable<string> unghighlitherParts = pathParts.Take(pathParts.Length - 1);
            string unghighlightedString = string.Join(@"/", unghighlitherParts); 
            Console.Write("\t" + unghighlightedString + @"/");

            // Superproject name is highlithed
            string superProjectName = pathParts[pathParts.Length - 1];
            CustomConsole.WriteLineColored(superProjectName, TextType.ImporantText);
        }


        CustomConsole.WriteColored("Push to remote: ", TextType.MundaneText);
        CustomConsole.WriteColored(_userConfigFacade.PushingToRemote.ToString(), _userConfigFacade.PushingToRemote ? ConsoleColor.DarkGreen : ConsoleColor.DarkGray);
        CustomConsole.WriteColored(" // Ak False, aplikacia ma zakazane pushovat na remote", TextType.MundaneText);

        CustomConsole.WriteLineColored(Environment.NewLine + Environment.NewLine + "Data saved at: C:\\Users\\{user}\\AppData\\Roaming", TextType.MundaneText);
        Console.WriteLine();
    }

    private static int PrintMenuOptionsAndGetTheirCount()
    {
        List<string> menuOptions =
        [
            "0. Pridat superprojekt",
            "1. Zmazat superprojekt",
            "4. Toggle pushovanie na remote",
            "3. Spat do hlavneho menu",
        ];

        foreach(string menuOption in menuOptions)
        {
            Console.WriteLine(menuOption);
        }

        return menuOptions.Count;
    }
}