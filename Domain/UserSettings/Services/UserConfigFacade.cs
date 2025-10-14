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
    private UserConfig _userConfig;

    private const string ConfigFileName = "SubmoduleTrackerConfig.txt";
    private readonly string ConfigFilePath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\{ConfigFileName}";

    public List<ConfigSuperProject> ConfigSuperProjects => _userConfig.SuperProjects;

    public readonly List<MetaSuperProject> MetaSupeprojects;

    public List<string> RelevantBranches => _userConfig.RelevantBranches;

    public bool PushingToRemote => _userConfig.PushingToRemote;

    public UserConfigFacade()
    {
        _userConfig = LoadUserConfig();

        MetaSupeprojects = _userConfig.SuperProjects
            .Select(x => new MetaSuperProject(x.WorkingDirectory))
            .ToList();
    }

    public VoidResult AddSuperproject(string superProjectWorkdir) 
    {
        string? validSuperprojectPath = TryGetGitRepositoryWorkingDirectory(superProjectWorkdir);

        if (validSuperprojectPath == null)
        {
            // invalid path
            return VoidResult.WithFailure("Na zadanej ceste sa nenachadza git repozitar");
        }

        // We exclude '\' from comparison There would be different count of '\' for path in newly added superproject and path added by user would be inconsisent. 
        if (_userConfig.ContainsSuperproject(superProjectWorkdir))
        {
            return VoidResult.WithFailure("Superprojekt uz je pridany");
        }

        _userConfig.SuperProjects.Add(new ConfigSuperProject() { WorkingDirectory = validSuperprojectPath });

        return SaveOptions()
            ? VoidResult.WithSuccess()
            : VoidResult.WithFailure("Was not able to save the User Config");
    }

    public VoidResult DeleteSuperProject(int superProjectIndex)
    {
        _userConfig.SuperProjects.RemoveAt(superProjectIndex);

        return SaveOptions()
            ? VoidResult.WithSuccess()
            : VoidResult.WithFailure("Was not able to save the User Config");
    }

    /// <summary>
    /// Serialize and and save new version of <see cref="_userConfig"/>
    /// </summary>
    private bool SaveOptions()
    {
        string stringyUserConfig = JsonSerializer.Serialize(_userConfig);

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
    /// Validates whether is valid Git repository and return path
    /// </summary>
    /// <returns>Returns working directory if there is a Git repository at path, null otherwise </returns>
    private static string? TryGetGitRepositoryWorkingDirectory(string superprojectWorkdir)
    {
        try
        {
            Repository superProjectGitRepository = new(superprojectWorkdir);
            // workdir in Repository object is trimmed of excessive chars
            string clearedWorkingDirectory = superProjectGitRepository.Info.WorkingDirectory;

            if(clearedWorkingDirectory.EndsWith('\\'))
            {
                clearedWorkingDirectory = clearedWorkingDirectory.Substring(0, clearedWorkingDirectory.Length - 1);
            }

            return clearedWorkingDirectory;
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