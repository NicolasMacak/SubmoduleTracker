using SubmoduleTracker.Core.CommonTypes.SuperProjects;
using SubmoduleTracker.Core.ConsoleTools;
using SubmoduleTracker.Core.ConsoleTools.Constants;
using SubmoduleTracker.Core.GitInteraction.CLI;
using SubmoduleTracker.Core.GitInteraction.Model;
using SubmoduleTracker.Core.Navigation.Services;
using SubmoduleTracker.Core.SubmoduleAlignmentTable;
using SubmoduleTracker.Domain.AlignmentExecution.Models;
using SubmoduleTracker.Domain.HomeScreen;
using SubmoduleTracker.Domain.UserSettings.Services;

namespace SubmoduleTracker.Domain.AlignmentExecution;
public class AlignmentExecutionWorkflow(UserConfigService userConfigFacade, NavigationService navigationService) : INavigable
{
    private const string DevBranch = "dev";
    private const string TestBranch = "test";

    private readonly UserConfigService _userConfigFacade = userConfigFacade;
    private readonly NavigationService _navigationService = navigationService;

    // alignment occurs after the feature has been tested and approved
    private readonly List<GitBranch> _alignmentRelevantBranches = [new GitBranch(TestBranch)];

    public void Run()
    {
        // Superprojects that contain submodule(selected by user in this method)
        string? selectedSubmodule = LetUserSelectSubmodule(_userConfigFacade.MetaSuperprojects);
        if (string.IsNullOrEmpty(selectedSubmodule))
        {
            _navigationService.NavigateTo<HomeScreenWorkflow>();
            return;
        }

        // superprojects that contain relevant submodule
        List<RobustSuperProject> relevantRobustSuperProjects =_userConfigFacade.MetaSuperprojects 
            .Where(x => x.SubmodulesNames.Contains(selectedSubmodule!))
            .Select(x => x.ToRobustSuperproject(_alignmentRelevantBranches))
            .ToList();

        SubmoduleAlignmentTablePrinter.PrintTableForSuperProjects(relevantRobustSuperProjects, _alignmentRelevantBranches, [selectedSubmodule!]);

        List<AligningSuperproject> superprojectsToAlign = GetSuperProjectsToAlign(relevantRobustSuperProjects, _alignmentRelevantBranches, selectedSubmodule!);

        if (superprojectsToAlign.Count == 0)
        {
            CustomConsole.WriteLineColored($"Alignment for subomdule {selectedSubmodule} is not required. Terminating execution.", TextType.Success);
            return;
        }

        if(!UserPrompts.AskYesOrNoQuestion("Forward Commit will be created at the TEST branch for all missaligned submodules. Then TEST branch will be merged into DEV branch. Continue?"))
        {
            return;
        }

        // Begin alignemnt process
        List<AligningSuperproject> successfullyAlignedSuperprojects = AlignSubmoduleInSuperprojects(selectedSubmodule!, superprojectsToAlign);

        if (!ShouldOperationContinue(successfullyAlignedSuperprojects))
        {
            return;
        }

        foreach (AligningSuperproject superproject in successfullyAlignedSuperprojects) // create forward commit for test and dev
        {
            foreach (string branchToPush in new List<string> { DevBranch, TestBranch })
            {
                GitCommandExecutor.Switch(superproject.Workdir, branchToPush);
                GitCommandExecutor.Push(superproject.Workdir);
            }
        }

        CustomConsole.WriteLineColored("Forward commits pushed to the remote. Alignment Completed. Terminating execution.", TextType.Success);
        _navigationService.PromptReturnToMainMenu();
    }

    private bool ShouldOperationContinue(List<AligningSuperproject> successfullyAlignedSuperprojects)
    {
        if (successfullyAlignedSuperprojects.Count == 0)
        {
            CustomConsole.WriteLineColored(Environment.NewLine + "Forward commit was not created for any superproject! Terminating execution.", TextType.Error);
            return false;
        }

        if (!_userConfigFacade.PushingToRemote)
        {
            CustomConsole.WriteColored(Environment.NewLine + "Forward commits created. Push to remote manually. Automatic push to remote can be set in User settings. Terminating execution.", TextType.Success);
            return false;
        }

        return UserPrompts.AskYesOrNoQuestion("Last step is to push successfully aligned submodules on branch DEV, TEST to the remote. Continue?");
    }

    /// <summary>
    /// Uses GIT to Forward submodule in superprojects
    /// </summary>
    /// <param name="submoduleToForward">Submodule to align</param>
    /// <param name="superprojectsToAlign">Superprojects to be aligned</param>
    /// <returns>Successfully forwarded superprojects</returns>
    private static List<AligningSuperproject> AlignSubmoduleInSuperprojects(string submoduleToForward, List<AligningSuperproject> superprojectsToAlign)
    {
        List<AligningSuperproject> successfullyAlignedSuperprojects = [];

        foreach (AligningSuperproject superproject in superprojectsToAlign)
        {
            try
            {
                // ALIGNING IN TEST
                string submoduleWorkdir = superproject.Workdir + @$"\{submoduleToForward}";

                // checkout branch in superproject
                GitCommandExecutor.Switch(superproject.Workdir, TestBranch);
                GitCommandExecutor.FetchAndFastForwardPull(superproject.Workdir);

                // checkout and pull submodule branch
                GitCommandExecutor.Switch(submoduleWorkdir, TestBranch); // checkout
                GitCommandExecutor.FetchAndFastForwardPull(submoduleWorkdir); // fetch and pull

                // Forward submodule in superproject
                GitCommandExecutor.CreateForwardCommit(superproject.Workdir, submoduleToForward);
                CustomConsole.WriteLineColored($"{superproject.Title}: Forward commit created on {TestBranch} branch.", TextType.Success);

                // MERGE TEST INTO DEV
                GitCommandExecutor.Switch(superproject.Workdir, DevBranch);
                GitCommandExecutor.Merge(superproject.Workdir, TestBranch);
                CustomConsole.WriteLineColored($"{superproject.Title}: {TestBranch} branch merged into {DevBranch}.", TextType.Success);

                successfullyAlignedSuperprojects.Add(superproject);
            }
            catch (Exception ex)
            {
                CustomConsole.WriteLineColored($"Failed to align superproject {superproject.Title}", TextType.Error);
                CustomConsole.WriteLineColored(ex.Message, TextType.Error);
            }
        }

        if (successfullyAlignedSuperprojects.Count != superprojectsToAlign.Count && successfullyAlignedSuperprojects.Count > 0)
        {
            CustomConsole.WriteErrorLine("Not all superprojects successfully aligned");
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

        int? selectedSubmoduleIndex = UserPrompts.GetIndexOfUserChoice(allSubmodules, "Choose an submodule to align", UserPrompts.ReturnToMainMenuPrompt);
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
        List<AligningSuperproject> superProjectsToAlign = [];

        foreach (RobustSuperProject superProject in relevantSuperprojects)
        {
            List<string> branchesToAlign = [];

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