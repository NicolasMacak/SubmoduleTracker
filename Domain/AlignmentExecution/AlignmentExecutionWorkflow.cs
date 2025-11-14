using SubmoduleTracker.Core.ConsoleTools;
using SubmoduleTracker.Core.GitInteraction.CLI;
using SubmoduleTracker.Core.GitInteraction.CommandExceptions;
using SubmoduleTracker.Core.GitInteraction.Model;
using SubmoduleTracker.Domain.UserSettings.Services;

namespace SubmoduleTracker.Domain.AlignmentExecution;
public class AlignmentExecutionWorkflow
{
    private readonly UserConfigFacade _userConfigFacade;

    public AlignmentExecutionWorkflow(UserConfigFacade userConfigFacade)
    {
        _userConfigFacade = userConfigFacade;
    }

    public void Run()
    {
        List<MetaSuperProject> allSuperprojects = _userConfigFacade.MetaSupeprojects;
        List<string> relevantBranches = _userConfigFacade.RelevantBranches;

        // Superprojects that contain submodule(selected by user in this method)
        string selectedSubmodule = LetUserSelectSubmodule(allSuperprojects);

        // superprojects that contain relevant submodule
        List<RobustSuperProject> relevantRobustSuperProjects = allSuperprojects
            .Where(x => x.SubmodulesNames.Contains(selectedSubmodule))
            .Select(x => x.ToRobustSuperproject(relevantBranches))
            .ToList();

        PrintSubmoduleAlignmentTableWorkflow.Run(selectedSubmodule, relevantRobustSuperProjects, relevantBranches);

        if (!ShouldAlignmentContinue(relevantRobustSuperProjects))
        {
            return;
        }

        List<AligningSuperproject> aligningSuperprojects = GetSuperProjectsToAlign(relevantRobustSuperProjects, relevantBranches);

        // Begin alignemnt process
        if (!AlignSuperprojects(selectedSubmodule, aligningSuperprojects))
        {
            return;
        }

        // Locally aligned. Last thing to do is push to remote
        PushToRemoteWithPermission(aligningSuperprojects);
    }

    private void PushToRemoteWithPermission(List<AligningSuperproject> superprojectsToAlign)
    {
        CustomConsole.WriteColored("Superprojekt a branche ktore budu zarovnane:", PredefinedColor.ImporantText);
        foreach (AligningSuperproject superproject in superprojectsToAlign)
        {
            CustomConsole.WriteColored(superproject.Title, PredefinedColor.MundaneText);
            foreach(string branch in superproject.branchesToAlign)
            {
                CustomConsole.WriteColored(branch, PredefinedColor.MundaneText);
            }
        }

        CustomConsole.WriteColored("Forward commity boli vytvorene", PredefinedColor.ImporantText);

        if (!_userConfigFacade.PushingToRemote)
        {
            CustomConsole.WriteColored("Push na remote zakazany. Mozete zmenit v nastaveniach. Ukoncujem exekuciu.", PredefinedColor.ImporantText);
            return;
        }

        bool approvedByUser = CustomConsole.AskYesOrNoQuestion("Push na remote?");

        if(!approvedByUser)
        {
            Console.WriteLine("Proces zastaveny uzivatelom");
            return;
        }

        PushAlignedSuperprojects(superprojectsToAlign);
    }

    /// <summary>
    /// Push aligned superproject to remote
    /// </summary>
    private static void PushAlignedSuperprojects(List<AligningSuperproject> superprojectsToAlign)
    {
        foreach(var superproject in superprojectsToAlign)
        {
            // Push main repo
            GitFacade.Push(superproject.Workdir);
        }
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
            CustomConsole.WriteColored("Nothing to Align! Stopping execution.", ConsoleColor.DarkGreen);
            return false;
        }

        return CustomConsole.AskYesOrNoQuestion("Pre nezarovnane branche budu vytvorene forward commity. Pokracovat?");
    }

    /// <summary>
    /// Uses GIT to Forward submodule in superprojects
    /// </summary>
    /// <param name="submoduleToForward">Submodule to align</param>
    /// <param name="superprojectsToAlign">Superprojects to be aligned</param>
    /// <returns>True if Forwarding commit was created for every superproject to align, false otherwise</returns>
    private static bool AlignSuperprojects(string submoduleToForward, List<AligningSuperproject> superprojectsToAlign)
    {
        try
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
                }
            }

            return true;
        }
        catch (CommandExecutionException ex)
        {
            // print
            CustomConsole.WriteErrorLine(ex.Message);
            return false;
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
                superProjectsToAlign.Add(new AligningSuperproject(superProject.Name, superProject.WorkingDirectory, branchesToAlign));
            }
        }

        return superProjectsToAlign;
    }
    
    private record AligningSuperproject(string Title, string Workdir, List<string> branchesToAlign);
}