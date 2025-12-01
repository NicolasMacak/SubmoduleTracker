using SubmoduleTracker.Core.ConsoleTools;
using SubmoduleTracker.Core.GitInteraction.CLI;
using SubmoduleTracker.Core.GitInteraction.Model;
using SubmoduleTracker.Domain.AlignmentExecution.Models;
using SubmoduleTracker.Domain.HomeScreen;
using SubmoduleTracker.Domain.Navigation;
using SubmoduleTracker.Domain.UserSettings.Services;

namespace SubmoduleTracker.Domain.AlignmentExecution;
public class AlignmentExecutionWorkflow : IWorkflow
{
    private readonly UserConfigService _userConfigFacade;
    private readonly NavigationService _navigationService;

    // For alignment, this is only possible option for now
    private readonly List<GitBranch> _alignmentRelevantBranches = new() { new GitBranch("dev"), new GitBranch("test") };

    public AlignmentExecutionWorkflow(UserConfigService userConfigFacade, NavigationService navigationService)
    {
        _userConfigFacade = userConfigFacade;
        _navigationService = navigationService;
    }

    public void Run()
    {
        // Superprojects that contain submodule(selected by user in this method)
        string? selectedSubmodule = LetUserSelectSubmodule(_userConfigFacade.MetaSuperprojects);
        if (string.IsNullOrEmpty(selectedSubmodule))
        {
            _navigationService.NavigateTo<HomeScreenWorkflow>();
        }

        // superprojects that contain relevant submodule
        List<RobustSuperProject> relevantRobustSuperProjects =_userConfigFacade.MetaSuperprojects 
            .Where(x => x.SubmodulesNames.Contains(selectedSubmodule!))
            .Select(x => x.ToRobustSuperproject(_alignmentRelevantBranches))
            .ToList();

        SubmoduleAlignmentTablePrinter.PrintTable(selectedSubmodule!, relevantRobustSuperProjects, _alignmentRelevantBranches);

        List<AligningSuperproject> superprojectsToAlign = GetSuperProjectsToAlign(relevantRobustSuperProjects, _alignmentRelevantBranches, selectedSubmodule!);

        if (superprojectsToAlign.Count == 0)
        {
            CustomConsole.WriteLineColored($"Zarovnanie pre submodul {selectedSubmodule} nie je potrebne. Ukoncujem exekuciu.", TextType.Success);
            return;
        }

        if(!CustomConsole.AskYesOrNoQuestion("Nasleduje vytvorenie forward commitov pre nezarovnané superprojekty. Pokracovat?"))
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
            CustomConsole.WriteLineColored(Environment.NewLine + "Forward commit nebol uspesne vytvoreny pre ziadny projekt! Ukoncujem exekuciu.", TextType.Error);
            return false;
        }

        if (!_userConfigFacade.PushingToRemote)
        {
            CustomConsole.WriteColored(Environment.NewLine + "Forward commity vytvorene. Push to remote yourself. Automatic push to remote can be set in User settings. Terminating execution.", TextType.Success);
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

                foreach (string branchToAlign in superproject.BranchesToAlign)
                {
                    // checkout branch in superproject
                    GitFacade.Switch(superproject.Workdir, branchToAlign);
                    GitFacade.FetchAndFastForwardPull(superproject.Workdir);

                    // checkout and pull submodule branch
                    GitFacade.Switch(submoduleWorkdir, branchToAlign); // checkout
                    GitFacade.FetchAndFastForwardPull(submoduleWorkdir); // fetch and pull

                    // Forward submodule in superproject
                    GitFacade.CreateForwardCommit(superproject.Workdir, submoduleToForward);
                    CustomConsole.WriteLineColored($"{superproject.Title}: {branchToAlign}. Forward commit vytvoreny.", TextType.Success);
                    successfullyAlignedSuperprojects.Add(superproject);
                }
            }
            catch (Exception ex)
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
    /// <returns>
    /// Name of the submodule to align. Null when user want's to go back to the main menu
    /// </returns>
    private static string? LetUserSelectSubmodule(List<MetaSuperProject> allSuperprojects)
    {
        // List of all submodules
        List<string> allSubmodules =
            allSuperprojects.SelectMany(x => x.SubmodulesNames, (superProject, submodule) => submodule)
            .Distinct() // Made unique
            .ToList();

        int? selectedSubmoduleIndex = CustomConsole.GetIndexOfUserChoice(allSubmodules, "Vyberte submodule na zarovnanie", "Zadajte \"\" ak sa chcete vratit do hlavneho menu");
        // User entered empty string
        if (!selectedSubmoduleIndex.HasValue)
        {
            return null;
        }

        return allSubmodules.ElementAt(selectedSubmoduleIndex!.Value);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="relevantSuperprojects"></param>
    /// <param name="relevantBranches"></param>
    /// <param name="selectedSubmodule"></param>
    /// <returns></returns>
    private static List<AligningSuperproject> GetSuperProjectsToAlign(List<RobustSuperProject> relevantSuperprojects, List<GitBranch> relevantBranches, string selectedSubmodule)
    {
        List<AligningSuperproject> superProjectsToAlign = new();

        foreach (RobustSuperProject superProject in relevantSuperprojects)
        {
            List<string> branchesToAlign = new();

            foreach(GitBranch branch in relevantBranches)
            {
                string superProjectBranchCommitIndex = superProject.IndexCommitRefs[branch.RemoteName][selectedSubmodule]; // where submodule points on in this branch
                string submoduleBranchHeadCommit = superProject.HeadCommitRefs[branch.RemoteName][selectedSubmodule]; // HEAD commit on this branch

                // Does submodule in superproject points to the HEAD commit?
                if(superProjectBranchCommitIndex != submoduleBranchHeadCommit)
                {
                    branchesToAlign.Add(branch.LocalName); // add branch to align
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
}