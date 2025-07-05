using SubmoduleTracker.Extensions;
using SubmoduleTracker.Model;

namespace SubmoduleTracker.Managers;
public sealed class SuperprojectsManager
{
    public Dictionary<string, SuperProject> SuperProjects = new();

    /// <summary>
    /// List of all submodules of all superprojects
    /// </summary>
    // <name, path>
    public Dictionary<string, string> AllSubmodulesWithPaths = new();

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

            foreach (string submoduleName in superProject.SubmodulesNames)
            {
                AllSubmodulesWithPaths.TryAdd(submoduleName, @$"{superProjectPath}\{submoduleName}");
            }
        }
    }

    public async Task<SuperProject> GetSuperProjectPopulatedWithBranchesSubmodules(string superProjecstName)
    {
        SuperProject requestedSuperProject = SuperProjects[superProjecstName];

        await requestedSuperProject.PopulateSubmoduleReferencesForBranches(_relevantBranches);

        return requestedSuperProject;
    }

    public async Task<List<SuperProject>> GetFullSuperProjectList()
    {
        throw new NotImplementedException();
    }

}