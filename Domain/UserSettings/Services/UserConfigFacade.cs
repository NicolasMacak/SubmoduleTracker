using System.Text.Json;
using LibGit2Sharp;
using SubmoduleTracker.Core.GitInteraction.Model;
using SubmoduleTracker.Core.Result;
using SubmoduleTracker.Domain.UserSettings.Model;

namespace SubmoduleTracker.Domain.UserSettings.Services;

// File location
// C:\Users\macak\AppData\Roaming
public sealed class UserConfigFacade
{
    private UserConfig UserConfig;

    private const string ConfigFileName = "SubmoduleTrackerConfig.txt";
    private readonly string ConfigFilePath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\{ConfigFileName}";

    public List<ConfigSuperProject> ConfigSuperProjects => UserConfig.SuperProjects;

    public List<MetaSuperProject> MetaSupeprojects => UserConfig.SuperProjects
        .Select(x => new MetaSuperProject(x.WorkingDirectory))
        .ToList();

    public bool PushingToRemote => UserConfig.PushingToRemote;

    public UserConfigFacade()
    {
        UserConfig = LoadUserConfig();
    }

    public OperationResult AddSuperproject(string superProjectWorkdir) 
    {
        string? validSuperprojectPath = TryGetValidWorkingDirectory(superProjectWorkdir);

        if (validSuperprojectPath == null)
        {
            // invalid path
            return OperationResult.WithFailure("Na zadanej ceste sa nenachadza git repozitar");
        }

        // We exclude '\' from comparison There would be different count of '\' for path in newly added superproject and path added by user would be inconsisent. 
        if (UserConfig.ContainsSuperproject(superProjectWorkdir))
        {
            return OperationResult.WithFailure("Superprojekt uz je pridany");
        }

        UserConfig.SuperProjects.Add(new ConfigSuperProject(superProjectWorkdir));

        return SaveOptions()
            ? OperationResult.WithSuccess()
            : OperationResult.WithFailure("Was not able to save the User Config");
    }

    public OperationResult DeleteSuperProject(int superProjectIndex)
    {
        UserConfig.SuperProjects.RemoveAt(superProjectIndex);

        return SaveOptions()
            ? OperationResult.WithSuccess()
            : OperationResult.WithFailure("Was not able to save the User Config");
    }

    /// <summary>
    /// Serialize and and save new version of <see cref="UserConfig"/>
    /// </summary>
    private bool SaveOptions()
    {
        string stringyUserConfig = JsonSerializer.Serialize(UserConfig);

        try
        {
            File.WriteAllText(ConfigFilePath, stringyUserConfig);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return false;
        }
    }


    /// <summary>
    /// Validates directory entered by user
    /// </summary>
    /// <returns>Returns cleaned working directory if success, null otherwise </returns>
    private static string? TryGetValidWorkingDirectory(string superprojectWorkdir)
    {
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


    private UserConfig LoadUserConfig()
    {
        // File exits. Deserialize and return
        if (File.Exists(ConfigFilePath))
        {
            try
            {
                string serializedUserConfig = File.ReadAllText(ConfigFilePath);

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
                Console.WriteLine($"You can find him at {ConfigFilePath}");
                throw;
            }
        }
        // Config has not been saved. Return empty User Config
        else
        {
            return new UserConfig();
        }
    }

}