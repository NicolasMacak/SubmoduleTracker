using System.Text.Json;
using LibGit2Sharp;
using SubmoduleTracker.Core.CommonTypes.Result;
using SubmoduleTracker.Core.CommonTypes.SuperProjects;
using SubmoduleTracker.Domain.UserSettings.Model;

namespace SubmoduleTracker.Domain.UserSettings.Services;

// File location
// C:\Users\{user}\AppData\Roaming
public sealed class UserConfigService
{
    private readonly UserConfig _userConfig;

    private const string ConfigFileName = "SubmoduleTrackerConfig.txt";
    private readonly string ConfigFilePath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\{ConfigFileName}";

    public List<MetaSuperProject> MetaSuperprojects;

    public bool PushingToRemote => _userConfig.PushingToRemote;

    public UserConfigService()
    {
        _userConfig = LoadUserConfig();

        MetaSuperprojects = CreateMetaSuperprojectsFromWorkdirs();
    }

    private List<MetaSuperProject> CreateMetaSuperprojectsFromWorkdirs()
    {
        return _userConfig.SuperProjectsWorkdirs
            .Select(superProjectWorkdir => new MetaSuperProject(superProjectWorkdir))
            .ToList();
    }

    public NonModelResult AddSuperproject(string superProjectWorkdir) 
    {
        if (string.IsNullOrEmpty(superProjectWorkdir))
        {
            return NonModelResult.WithFailure("Superproject workdir can not be null");
        }

        ModelResult<string> addedSuperProjectWorkdirResult = TryGetGitRepositoryWorkingDirectory(superProjectWorkdir);

        if (addedSuperProjectWorkdirResult.ResultCode != ResultCode.Success)
        {
            // invalid path
            return NonModelResult.WithFailure(addedSuperProjectWorkdirResult.ErrorMessage!);
        }

        // We exclude '\' from comparison There would be different count of '\' for path in newly added superproject and path added by user would be inconsisent. 
        if (_userConfig.ContainsSuperproject(addedSuperProjectWorkdirResult.Model!))
        {
            return NonModelResult.WithFailure($"Superproject at path {addedSuperProjectWorkdirResult.Model} already added!");
        }

        _userConfig.SuperProjectsWorkdirs.Add(addedSuperProjectWorkdirResult.Model!);

        MetaSuperprojects = CreateMetaSuperprojectsFromWorkdirs(); // Collection changed. Recreate

        return SaveOptions()
            ? NonModelResult.WithSuccess()
            : NonModelResult.WithFailure("Was not able to save the User Config");
    }

    public NonModelResult TogglePushingToRemote()
    {
        _userConfig.PushingToRemote = !_userConfig.PushingToRemote;

        return SaveOptions()
            ? NonModelResult.WithSuccess()
            : NonModelResult.WithFailure("Was not able to save the User Config");
    }

    public NonModelResult DeleteSuperProject(int superProjectIndex)
    {
        _userConfig.SuperProjectsWorkdirs.RemoveAt(superProjectIndex);

        MetaSuperprojects = CreateMetaSuperprojectsFromWorkdirs(); // Collection changed. Recreate

        return SaveOptions()
            ? NonModelResult.WithSuccess()
            : NonModelResult.WithFailure("Was not able to save the User Config");
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
    private static ModelResult<string> TryGetGitRepositoryWorkingDirectory(string superprojectWorkdir)
    {
        ModelResult<string> result = new();
        try
        {
            Repository gitRepository = new(superprojectWorkdir);
            if(gitRepository.Submodules.Count() == 0)
            {
                return result.WithFailure("Repository must contain submodules!");
            }

            // workdir in Repository object is trimmed of excessive chars
            string clearedWorkingDirectory = gitRepository.Info.WorkingDirectory;

            if (clearedWorkingDirectory.EndsWith('\\'))
            {
                clearedWorkingDirectory  = clearedWorkingDirectory.Substring(0, clearedWorkingDirectory .Length - 1);
            }

            return result.WithSuccess(Path.GetFullPath(clearedWorkingDirectory));
        }
        catch (LibGit2SharpException)
        {
            return result.WithFailure("No repository found on path");
        }
        catch (Exception ex)
        {
            return result.WithFailure(ex.Message);
        }
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