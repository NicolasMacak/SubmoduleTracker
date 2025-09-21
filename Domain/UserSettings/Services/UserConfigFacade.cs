using System.Text.Json;
using LibGit2Sharp;
using SubmoduleTracker.Core.Result;
using SubmoduleTracker.Domain.UserSettings.Model;

namespace SubmoduleTracker.Domain.UserSettings.Services;
public sealed class UserConfigFacade
{
    private UserConfig UserConfig;

    private const string ConfigFileName = "SubmoduleTrackerConfig.txt";
    private readonly string ConfigFilePath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\{ConfigFileName}";

    public List<SuperProjectConfig> SuperProjectsConfig => UserConfig.SuperProjects;
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
            return new OperationResult("Na zadanej ceste sa nenachadza git repozitar");
        }

        // We exclude '\' from comparison There would be different count of '\' for path in newly added superproject and path added by user would be inconsisent. 
        if (UserConfig.ContainsSuperproject(superProjectWorkdir))
        {
            return new OperationResult("Superprojekt uz je pridany");
        }

        UserConfig.SuperProjects.Add(new SuperProjectConfig(superProjectWorkdir));

        SaveOptions(userConfig);
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



    private void SaveUserConfigToFile()
    {

    }
}