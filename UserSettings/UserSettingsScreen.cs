using System.Text.Json;
using LibGit2Sharp;
using SubmoduleTracker.ConsoleTools;
using SubmoduleTracker.UserSettings.Model;

namespace SubmoduleTracker.UserSettings;

public static class UserSettingsScreen
{
    // C:\Users\macak\AppData\Roaming
    private const string ConfigFileName = "SubmoduleTrackerConfig.txt";

    public static void Main(UserConfig userConfig, string? errorMessage = null)
    {
        if (!string.IsNullOrEmpty(errorMessage))
        {
            CustomConsole.WriteErrorLine(errorMessage + Environment.NewLine);
        }

        PrintUserConfig(userConfig);

        int menuOptionsCount = PrintMenuOptionsAndGetTheirCount();
        string? choice = Console.ReadLine();

        int? validatedChoice = ConsoleValidation.ReturnValidatedNumberOption(choice, menuOptionsCount);
        if (!validatedChoice.HasValue)
        {
            Console.WriteLine($"Invalid input. Must be number from 1 to {menuOptionsCount}");
        }

        switch (validatedChoice!.Value) {
            case 1:
                // Add Superproject to the userConfig and save
                TryAddingNewSuperproject(userConfig);
                break;

            case 2: DeleteSuperproject(userConfig); break;
            default: Console.WriteLine("No such option!"); break;
        }
    }

    public static UserConfig GetUserConfiguration()
    {
        string configFilePath = GetConfigFilePath();

        // File exits. Deserialize and return
        if (File.Exists(configFilePath))
        {
            try
            {
                string serializedUserConfig = File.ReadAllText(configFilePath);

                // user cleared the config. we return empty one
                if (string.IsNullOrEmpty(serializedUserConfig))
                {
                    return new UserConfig();
                }

                UserConfig? deserializedUserConfig = JsonSerializer.Deserialize<UserConfig>(serializedUserConfig);

                if (deserializedUserConfig == null)
                {
                    throw new ArgumentNullException(nameof(deserializedUserConfig));
                }

                return deserializedUserConfig;
            }
            catch (Exception)
            {
                Console.WriteLine("Error deserializing the userconfig");
                Console.WriteLine("You can start over by deleting or clear the file");
                Console.WriteLine($"You can find him at {configFilePath}");
                throw;
            }
        }
        // Config has not been saved. Return empty User Config
        else
        {
            return new UserConfig();
        }
    }

    /// <summary>
    /// Adds new superproject to userConfig. When error occurs. Called recurrsivelly until success or cancel by user
    /// </summary>
    /// <param name="userConfig"></param>
    /// <param name="errorMessage">Contains error message from last attempt</param>
    private static void TryAddingNewSuperproject(UserConfig userConfig, string? errorMessage = null)
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
            Main(userConfig);
        }

        string? validSuperprojectPath = TryGetValidWorkingDirectory(superprojectWorkdir!);

        if (validSuperprojectPath == null)
        {
            // invalid path
            TryAddingNewSuperproject(userConfig, "Na zadanej ceste sa nenachadza git repozitar");
        }

        // We exclude '\' from comparison There would be different count of '\' for path in newly added superproject and path added by user would be inconsisent. 
        if (userConfig.SuperProjects.Any(x => x.WorkingDirectory.Replace(@"\", string.Empty) == superprojectWorkdir!.Replace(@"\", string.Empty)))
        {
            // Such superproject already added
            TryAddingNewSuperproject(userConfig, $"Superproject on path {superprojectWorkdir} already added");
        }

        userConfig.SuperProjects.Add(new SuperProjectConfig(superprojectWorkdir!));

        SaveOptions(userConfig);
    }

    /// <summary>
    /// Validates directory entered by user
    /// </summary>
    /// <returns>Returns cleaned working directory if success, null otherwise </returns>
    private static string? TryGetValidWorkingDirectory(string superprojectWorkdir)
    {
        // "C:\\NON_SYSTEM\\Submodule-C";

        try
        {
            Repository superProjectGitRepository = new(superprojectWorkdir); 
            return superProjectGitRepository.Info.WorkingDirectory; // workdir in Repository object is trimmed of bs chars
        }
        catch (LibGit2SharpException)
        {
            return null; // repository not found on path
        }
        catch (Exception ex)
        {
            Console.WriteLine("Neznáma chyba. Kontaktujte Joška Vajdu.");
            Console.WriteLine(ex.Message);
        }

        return null;
    }

    /// <summary>
    /// Remove superproject from config
    /// </summary>
    private static void DeleteSuperproject(UserConfig userConfig, string? errorMessage = null)
    {
        if (!string.IsNullOrEmpty(errorMessage))
        {
            CustomConsole.WriteErrorLine(errorMessage + Environment.NewLine);
        }

        Console.WriteLine(Environment.NewLine + "Choose superproject to delete" + Environment.NewLine);
        Console.WriteLine("Enter empty string for step back" + Environment.NewLine);

        // Print deletion options
        for(int i = 0; i < userConfig.SuperProjects.Count; i++)
        {
            Console.WriteLine($"{i}. {userConfig.SuperProjects[i].WorkingDirectory}");
        }

        string? choice = Console.ReadLine();

        // step back
        if (string.IsNullOrEmpty(choice))
        {
            Main(userConfig);
        }

        int? option = ConsoleValidation.ReturnValidatedNumberOption(choice, userConfig.SuperProjects.Count - 1, 0);

        if (!option.HasValue)
        {
            DeleteSuperproject(userConfig, $"Invalid input. Valid options are from {0} to {userConfig.SuperProjects.Count - 1}");
        }

        userConfig.SuperProjects.RemoveAt(option!.Value);

        SaveOptions(userConfig);
    }

    /// <summary>
    /// Serialize and and save new version of <see cref="UserConfig"/>
    /// </summary>
    private static void SaveOptions(UserConfig userConfig)
    {
        string stringyUserConfig = JsonSerializer.Serialize(userConfig);
        File.WriteAllText(GetConfigFilePath(), stringyUserConfig);

        Console.Clear(); // We clear console only when operation ends in success
        Main(userConfig); 
    }

    private static void PrintUserConfig(UserConfig userConfig)
    {
        foreach(SuperProjectConfig superproject in userConfig.SuperProjects)
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

    private static string GetConfigFilePath()
    {
        return $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\{ConfigFileName}";
    }
}