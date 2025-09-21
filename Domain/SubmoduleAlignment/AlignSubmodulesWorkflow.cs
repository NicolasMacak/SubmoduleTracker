using SubmoduleTracker.Core.ConsoleTools;
using SubmoduleTracker.Core.GitInteraction.CLI;
using SubmoduleTracker.Core.GitInteraction.Model;
using SubmoduleTracker.Domain.UserSettings.Model;
using SubmoduleTracker.Domain.UserSettings.Services;

namespace SubmoduleTracker.Domain.SubmoduleAlignment;
public class AlignSubmodulesWorkflow
{
    private readonly UserConfigFacade _userConfigFacade;

    public AlignSubmodulesWorkflow(UserConfigFacade userConfigFacade)
    {
        _userConfigFacade = userConfigFacade;
    }

    public void Run()
    {
        List<MetaSuperProject> allSuperprojects = _userConfigFacade.MetaSupeprojects;

        List<string> relevantBranches = new() { "dev", "test" };

        // Superprojects that contain submodule(selected by user in this method)
        string selectedSubmodule = LetUserSelectSubmodule(allSuperprojects);

        // superprojects that contain relevant submodule
        List<RobustSuperProject> relevantRobustSuperProjects = allSuperprojects
            .Where(x => x.SubmodulesNames.Contains(selectedSubmodule))
            .Select(x => new RobustSuperProject(
                    name: x.Name,
                    workingDirectory: x.WorkingDirectory,
                    indexCommitRefs: x.GetSubmoduleIndexCommitsRefs(relevantBranches, relevantSubmodules: [selectedSubmodule]),
                    headCommitRefs: x.GetSubmoduleHeadCommitRefs(relevantBranches, relevantSubmodules: [selectedSubmodule])
                ))
            .ToList();

        PrintSubmoduleAlignmentTableWorkflow.Run(selectedSubmodule, relevantRobustSuperProjects, relevantBranches);

        if (!ShouldAlignmentContinue(relevantRobustSuperProjects))
        {
            return;
        }

        List<AligningSuperproject> aligningSuperprojects = GetSuperProjectsToAlign(relevantRobustSuperProjects, relevantBranches);

        // Begin alignemnt process
        AlignSuperprojects(selectedSubmodule, aligningSuperprojects);
    }

    /// <summary>
    /// Checks whether there is something to align and asks user for permission
    /// </summary>
    /// <returns>True when dissaligments exists and user grants permission to procceed. False otherwise</returns>
    private static bool ShouldAlignmentContinue(List<RobustSuperProject> relevantRobustSuperProjects)
    {
        // Nothing to align
        if(!relevantRobustSuperProjects.Any(x => x.GetDisalignemnts().Count > 0))
        {
            return false;
        }

        return CustomConsole.AskYesOrNoQuestion("Cervene submoduly budu zarovnane. Pokracovat?");
    }

    /// <summary>
    /// Uses GIT to Forward submodule in superprojects
    /// </summary>
    /// <param name="submoduleToForward">Submodule to align</param>
    /// <param name="superprojectsToAlign">Superprojects to be aligned</param>
    private static void AlignSuperprojects(string submoduleToForward, List<AligningSuperproject> superprojectsToAlign)
    {
        foreach (AligningSuperproject superproject in superprojectsToAlign)
        {
            string submoduleWorkdir = superproject.Workdir + @$"\{submoduleToForward}";

            foreach (string branchToAlign in superproject.branchesToAlign)
            {

                // checkout branch in superproject
                GitFacade.Switch(superproject.Workdir, branchToAlign);
                GitFacade.FetchAndPull(superproject.Workdir);

                // checkout and pull submodule branch
                GitFacade.Switch(submoduleWorkdir, branchToAlign); // checkout
                GitFacade.FetchAndPull(submoduleWorkdir); // fetch and pull

                // Forward submodule in superproject
                GitFacade.AddAndCommit(superproject.Workdir, submoduleToForward);

                if (/*SAFE MODE DISABLED*/true)
                {
                    GitFacade.Push(superproject.Workdir);
                }
            }
        }
    }

    /// <summary>
    /// Submodules from all superprojects are listed. User picks one. Superprojets that contain this submodule are returned
    /// </summary>
    /// <returns>Superprojects that contain submodule selected by user</returns>
    private static string LetUserSelectSubmodule(List<MetaSuperProject> allSuperprojects, string? errorMessage = null)
    {
        if (errorMessage != null)
        {
            CustomConsole.WriteErrorLine(errorMessage);
        }

        // List of all submodules
        List<string> allSubmodules =
            allSuperprojects.SelectMany(x => x.SubmodulesNames, (superProject, submodule) => submodule)
            .Distinct() // Unique list
            .ToList();

        for(int i = 0; i < allSubmodules.Count; i++)
        {
            Console.WriteLine($"{i}. {allSubmodules[i]}");
        }

        Console.WriteLine("Vyberte submodule na zarovnanie \n");

        string? stringChoice = Console.ReadLine();

        int? maybeNumberChoice = ConsoleValidation.ReturnValidatedNumberOption(stringChoice, allSubmodules.Count, 0);
        if (!maybeNumberChoice.HasValue)
        {
            LetUserSelectSubmodule(allSuperprojects, $"Invalid choice. Pick between 0 and {allSubmodules.Count-1}");
        }

        return allSubmodules.ElementAt(maybeNumberChoice!.Value);
    }
    
    private static List<AligningSuperproject> GetSuperProjectsToAlign(List<RobustSuperProject> relevantSuperprojects, List<string> relevantBranches)
    {
        List<AligningSuperproject> superProjectsToAlign = new();

        foreach (RobustSuperProject superProject in relevantSuperprojects)
        {
            List<string> branchesToAlign = new();

            foreach(string branch in relevantBranches)
            {
                string indexCommit = superProject.IndexCommitRefs[branch].First().Value; // where submodule points on in this branch
                string headCommit = superProject.HeadCommitRefs[branch].First().Value; // HEAD commit on this branch

                // Does submodule in superproject points to the HEAD commit?
                if(indexCommit != headCommit)
                {
                    branchesToAlign.Add(branch); // add branch to align
                }
            }

            // if collection has items, alignment is neccessary
            if (branchesToAlign.Count > 0)
            {
                superProjectsToAlign.Add(new AligningSuperproject(superProject.WorkingDirectory, branchesToAlign));
            }
        }

        return superProjectsToAlign;
    }
    
    private record AligningSuperproject(string Workdir, List<string> branchesToAlign);
}