using SubmoduleTracker.Core.ConsoleTools;
using SubmoduleTracker.Core.GitInteraction.Model;
using SubmoduleTracker.Core.Result;
using SubmoduleTracker.Domain.UserSettings.Services;

namespace SubmoduleTracker.Domain.UserSettings;

public class ManageUserSettingsWorkflow
{
    private readonly UserConfigFacade _userConfigFacade;

    public ManageUserSettingsWorkflow(UserConfigFacade userConfigFacade)
    {
        _userConfigFacade = userConfigFacade;
    }

    public void Run(string? errorMessage = null)
    {
        if (!string.IsNullOrEmpty(errorMessage))
        {
            CustomConsole.WriteErrorLine(errorMessage + Environment.NewLine);
        }

        PrintUserConfig();

        int menuOptionsCount = PrintMenuOptionsAndGetTheirCount();
        string? choice = Console.ReadLine();

        int? validatedChoice = ConsoleValidation.ReturnValidatedNumberOption(choice, menuOptionsCount);
        if (!validatedChoice.HasValue)
        {
            Console.Clear();
            Run($"Invalid input. Must be number from 1 to {menuOptionsCount}");
        }

        switch (validatedChoice!.Value) {
            case 1:
                // Add Superproject to the userConfig and save
                TryAddingNewSuperproject();
                break;

            case 2: DeleteSuperproject(); break;
            default: Console.WriteLine("No such option!"); break;
        }
    }

    /// <summary>
    /// Adds new superproject to userConfig. When error occurs. Called recurrsivelly until success or cancel by user
    /// </summary>
    /// <param name="userConfig"></param>
    /// <param name="errorMessage">Contains error message from last attempt</param>
    private void TryAddingNewSuperproject(string? errorMessage = null)
    {
        if (!string.IsNullOrEmpty(errorMessage)) {
            CustomConsole.WriteErrorLine(errorMessage + Environment.NewLine);
        }

        Console.WriteLine("Zadajte absolutnu cestu ku git repozitaru");
        Console.WriteLine("Pre krok spat zadajte empty string" + Environment.NewLine);

        string? superprojectWorkdir = Console.ReadLine();

        // Step back if user enters nothing
        if (string.IsNullOrEmpty(superprojectWorkdir))
        {
            Console.Clear();
            Run();
        }

        // user input is handled here
        VoidResult result = _userConfigFacade.AddSuperproject(superprojectWorkdir!);

        if (result.ResultCode != ResultCode.Success)
        {
            CustomConsole.WriteErrorLine(result.ErrorMessage + Environment.NewLine);
        }

        Console.Clear();
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

        Console.WriteLine(Environment.NewLine + "Choose superproject to delete" + Environment.NewLine);
        Console.WriteLine("Enter empty string for step back" + Environment.NewLine);

        List<ConfigSuperProject> superProjects = _userConfigFacade.ConfigSuperProjects;

        // Print deletion options
        for(int i = 0; i < superProjects.Count; i++)
        {
            Console.WriteLine($"{i}. {superProjects[i].WorkingDirectory}");
        }

        string? choice = Console.ReadLine(); // Todo. Improve input

        // step back
        if (string.IsNullOrEmpty(choice))
        {
            Console.Clear();
            Run();
        }

        int? indexToDeleteAt = ConsoleValidation.ReturnValidatedNumberOption(choice, superProjects.Count, 0);

        if (!indexToDeleteAt.HasValue)
        {
            DeleteSuperproject($"Invalid input. Valid options are from {0} to {superProjects.Count - 1}");
        }

        VoidResult result = _userConfigFacade.DeleteSuperProject(indexToDeleteAt!.Value);

        if (result.ResultCode != ResultCode.Success)
        {
            DeleteSuperproject(result.ErrorMessage);
        }

        Console.Clear();
        Run();
    }

    private void PrintUserConfig()
    {
        foreach(ConfigSuperProject superproject in _userConfigFacade.ConfigSuperProjects)
        {
            // We separate path by folders
            // last element is the name of the superproject(Which we want to highlight)
            string[] pathParts = superproject.WorkingDirectory.Split(@"\"); 

            // We deconstruct the path to separate if to parts that we want highlithed and parts that we dont
            IEnumerable<string> unghighlitherParts = pathParts.Take(pathParts.Length - 1);
            string unghighlightedString = string.Join(@"/", unghighlitherParts); 
            Console.Write(unghighlightedString + @"/");

            // Superproject name is highlithed
            string superProjectName = pathParts[pathParts.Length - 1];
            CustomConsole.WriteHighlighted(superProjectName + Environment.NewLine);
        }

        Console.WriteLine();
    }

    private static int PrintMenuOptionsAndGetTheirCount()
    {
        List<string> menuOptions =
        [
            "1. Pridat superprojekt",
            "2. Zmazat superprojekt",
            "3. Spat do hlavneho menu"
        ];

        foreach(string menuOption in menuOptions)
        {
            Console.WriteLine(menuOption);
        }

        return menuOptions.Count;
    }
}