using SubmoduleTracker.Model;

namespace SubmoduleTracker.Managers;
public sealed class SuperprojectsManager
{
    public Dictionary<string, SuperProject> SuperProjects = new();

    /// <summary>
    /// Branches in question
    /// </summary>
    private List<string> _relevantBranches;

    public SuperprojectsManager(List<string> superProjectsPaths, List<string> relevantBranches) {
        _relevantBranches = relevantBranches;

        foreach (var superProjectPath in superProjectsPaths)
        {
            SuperProject superProject = new (superProjectPath);
            SuperProjects.Add(superProject.Name, superProject); 
        }
    }

    /// <summary>
    /// Returns work directories for all submodules in all superprojects
    /// </summary>
    /// 
    /// <returns>
    /// Returns working directories of submodules of all projects
    /// </returns>
    /// 
    /// <remarks>
    /// Later used to get access to git repositories of submodules.
    /// We do not care which superproject submodule belongs. We want git data, those are same across all instances of the submodules.
    /// </remarks>
    public Dictionary<string, string> GetSubmodulesWorkdirectories() 
    {
        Dictionary<string, string> SubmodulesWorkdirectories = new();

        foreach (SuperProject superproject in SuperProjects.Values)
        {
            foreach (string submoduleName in superproject.SubmodulesNames)
            {
                SubmodulesWorkdirectories.TryAdd(submoduleName, @$"{superproject.RepositoryPath}\{submoduleName}");
            }
        }

        return SubmodulesWorkdirectories;
    }


    /// <summary>
    /// Returns SubmoduleMappings for superproject
    /// </summary>
    public async Task<BranchSubmoduleMap> GetSubmodulePointingsOfSuperProject(string superProjecstName)
    {
        SuperProject requestedSuperProject = SuperProjects[superProjecstName];

        return await requestedSuperProject.GetSubmodulePointings(_relevantBranches);
    }

    public async Task<List<SuperProject>> GetFullSuperProjectList()
    {
        throw new NotImplementedException();
    }

}