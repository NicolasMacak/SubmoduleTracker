using LibGit2Sharp;
using SubmoduleTracker.ConsoleTools;
using SubmoduleTracker.UserSettings.Model;

namespace SubmoduleTracker.UserSettings;
public static class UserSettingsScreen
{
    public static void Main(UserConfig userConfig)
    {
        PrintUserConfig(userConfig);
        List<string> options = GetOptions();

        string? choice = Console.ReadLine();

        int? validatedChoice = ConsoleValidation.ReturnValidatedNumberOption(choice, options.Count);
        if (!validatedChoice.HasValue)
        {
            Console.WriteLine($"Invalid input. Must be number from 1 to {options.Count}");
        }

        switch (validatedChoice!.Value) {
            case 1:
                (string superProjectWorkdir, List<string> submodules) newSuperproject = GetNewSuperproject();
                userConfig.SuperProjects.Add(new SuperProjectConfig(newSuperproject.superProjectWorkdir));
                // Este uloz submodule
                SaveOptions(userConfig);
                break; //
        }

    }

    private static (string superProjectWorkdir, List<string> submodules) GetNewSuperproject()
    {
        // "C:\\NON_SYSTEM\\Submodule-C";

        Console.WriteLine("Zadajte absolutnu cestu ku git repozitaru");
        string? superprojectWorkdir = Console.ReadLine();

        Repository superProjectGitRepository = new(superprojectWorkdir); // spadne to ked je nevalidny? treba validovat

        List<string> submoduleWorkdirs =
            superProjectGitRepository.Submodules.Select(submodule => $"{superprojectWorkdir}/{submodule.Name}")
            .ToList();

        return (superprojectWorkdir, submoduleWorkdirs);
    }
    private static void DeleteSuperproject()
    {

    }

    private static void SaveOptions(UserConfig userConfig)
    {
        // vyclearuj file
        // uloz nanovo
        Main(userConfig); // loopuj naspat na menu
    }

    private static void PrintUserConfig(UserConfig userConfig)
    {
        foreach(SuperProjectConfig superproject in userConfig.SuperProjects)
        {
            Console.WriteLine(superproject.WorkingDirectory);
        }
    }

    private static List<string> GetOptions()
    {
        return new()
        {
            "1. Pridat superprojekt",
            "2. Zmazat superprojekt",
            "3. Spat do hlavneho menu"
        };
    }

}