using SubmoduleTracker.Core.ConsoleTools;
using SubmoduleTracker.Core.GitInteraction.CLI;
using SubmoduleTracker.Core.GitInteraction.CommandExceptions;
using SubmoduleTracker.Core.GitInteraction.Model;
using SubmoduleTracker.Domain.Navigation;
using SubmoduleTracker.Domain.UserSettings.Services;

namespace SubmoduleTracker.Domain.AlignmentExecution;
public class AlignmentExecutionWorkflow : IWorkflow
{
    private readonly UserConfigFacade _userConfigFacade;

    public AlignmentExecutionWorkflow(UserConfigFacade userConfigFacade)
    {
        _userConfigFacade = userConfigFacade;
    }

    public void Run()
    {
        Console.Clear();
        // Superprojects that contain submodule(selected by user in this method)
        string selectedSubmodule = LetUserSelectSubmodule(_userConfigFacade.MetaSupeprojects);

        // superprojects that contain relevant submodule
        List<RobustSuperProject> relevantRobustSuperProjects =_userConfigFacade.MetaSupeprojects 
            .Where(x => x.SubmodulesNames.Contains(selectedSubmodule))
            .Select(x => x.ToRobustSuperproject(_userConfigFacade.RelevantBranches))
            .ToList();

        SubmoduleAlignmentTablePrinter.PrintTable(selectedSubmodule, relevantRobustSuperProjects, _userConfigFacade.RelevantBranches);

        List<AligningSuperproject> superprojectsToAlign = GetSuperProjectsToAlign(relevantRobustSuperProjects, _userConfigFacade.RelevantBranches);

        if (superprojectsToAlign.Count == 0)
        {
            CustomConsole.WriteLineColored($"Zarovnanie pre submodul {selectedSubmodule} nie je potrebne. Ukoncujem exekuciu.", TextType.Success);
            return;
        }

        if(!CustomConsole.AskYesOrNoQuestion("Nasleduje vytvorene forward commitov pre nezarovnané superprojekty. Pokracovat?"))
        {
            return;
        }

        // Begin alignemnt process
        List<AligningSuperproject> successfullyAlignedSuperprojects = CreateForwardCommitsForSuperprojects(selectedSubmodule, superprojectsToAlign);

        if (!ShouldOperationContinue(superprojectsToAlign, successfullyAlignedSuperprojects))
        {
            return;
        }

        foreach (AligningSuperproject superproject in successfullyAlignedSuperprojects)
        {
            GitFacade.Push(superproject.Workdir);
        }
    }

    private bool ShouldOperationContinue(List<AligningSuperproject> superprojectsToAlign, List<AligningSuperproject> successfullyAlignedSuperprojects)
    {
        if (successfullyAlignedSuperprojects.Count == 0)
        {
            CustomConsole.WriteLineColored(Environment.NewLine + "Ziaden superproject sa nepodarilo zaronat! Ukoncujem exekuciu.", TextType.Error);
            return false;
        }

        if (!_userConfigFacade.PushingToRemote)
        {
            CustomConsole.WriteColored(Environment.NewLine + "Forward commity vytvorene. Ukoncujem exekuciu. Push na remote zakazany. Mozno zmenit v nastaveniach.", TextType.Success);
            return false;
        }

        return CustomConsole.AskYesOrNoQuestion("Pushnut uspesne zarovnane superprojekty na remote?");
    }

    /// <summary>
    /// Uses GIT to Forward submodule in superprojects
    /// </summary>
    /// <param name="submoduleToForward">Submodule to align</param>
    /// <param name="superprojectsToAlign">Superprojects to be aligned</param>
    /// <returns>Successfully forwarded superprojects</returns>
    private static List<AligningSuperproject> CreateForwardCommitsForSuperprojects(string submoduleToForward, List<AligningSuperproject> superprojectsToAlign)
    {
        List<AligningSuperproject> successfullyAlignedSuperprojects = new();

        foreach (AligningSuperproject superproject in superprojectsToAlign)
        {
            try
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
                    CustomConsole.WriteLineColored($"Forward commit vytvoreny. Superprojekt: {superproject.Title} Branch: {branchToAlign}", TextType.Success);
                    successfullyAlignedSuperprojects.Add(superproject);
                }
            }
            catch (CommandExecutionException ex)
            {
                CustomConsole.WriteLineColored($"Failed to align superproject {superproject.Title}", TextType.Error);
                CustomConsole.WriteLineColored(ex.Message, TextType.Error);
            }
        }

        if (successfullyAlignedSuperprojects.Count != superprojectsToAlign.Count && successfullyAlignedSuperprojects.Count > 0)
        {
            CustomConsole.WriteErrorLine("Nie vsetky superprojekty sa podarilo zarovnat");
        }

        return successfullyAlignedSuperprojects;
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
                superProjectsToAlign.Add(new AligningSuperproject(superProject.Name, superProject.WorkingDirectory, branchesToAlign));
            }
        }

        return superProjectsToAlign;
    }
    
    private record AligningSuperproject(string Title, string Workdir, List<string> branchesToAlign);
}