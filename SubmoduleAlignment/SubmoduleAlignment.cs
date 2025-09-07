using SubmoduleTracker.ConsoleTools;
using SubmoduleTracker.GitInteraction.CLI;
using SubmoduleTracker.GitInteraction.Model;
using SubmoduleTracker.UserSettings.Model;

namespace SubmoduleTracker.SubmoduleAlignment;
public static class SubmoduleAlignment
{
    public static void Index(UserConfig userConfig)
    {
        List<MetaSuperProject> allSuperprojcts = GetAllSuperprojects(userConfig.SuperProjects);

        List<string> relevantBranches = new() { "dev", "test" };

        // Superprojects that contain submodule(selected by user in this method)
        string selectedSubmodule = LetUserSelectSubmodule(allSuperprojcts);

        List<RobustSuperProject> relevantRobustSuperProjects = allSuperprojcts
            .Where(x => x.SubmodulesNames.Contains(selectedSubmodule))
            .Select(x => new RobustSuperProject(
                    name: x.Name,
                    workingDirectory: x.WorkingDirectory,
                    submoduleNames: x.SubmodulesNames,
                    indexCommitRefs: x.GetSubmoduleIndexCommitsRefs(relevantBranches, new List<string> { selectedSubmodule }),
                    headCommitRefs: x.GetSubmoduleHeadCommitRefs(relevantBranches, new List<string> { selectedSubmodule })
                ))
            .ToList();

        SubmoduleAlignmentInfoTable.PrintTable(selectedSubmodule, relevantRobustSuperProjects, relevantBranches);
        List<AligningSuperproject> superprojectsToAlign = GetSuperProjectsToAlign(selectedSubmodule, relevantRobustSuperProjects, relevantBranches);

        // Begin alignemnt process
        AlignSuperprojects(selectedSubmodule, superprojectsToAlign);
    }

    private static List<MetaSuperProject> GetAllSuperprojects(List<SuperProjectConfig> superProjectConfigs)
    {
        List<MetaSuperProject> allSuperprojects = new();

        foreach(SuperProjectConfig superProjectConfig in superProjectConfigs)
        {
            MetaSuperProject superproject = new(superProjectConfig.WorkingDirectory);
            allSuperprojects.Add(superproject);
        }

        return allSuperprojects;
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
    
    private static List<AligningSuperproject> GetSuperProjectsToAlign(string selectedSubmodule, List<RobustSuperProject> relevantSuperprojects, List<string> relevantBranches)
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
    
    private static void AlignSuperprojects(string submoduleToAlign, List<AligningSuperproject> superprojectsToAlign)
    {
        foreach (AligningSuperproject superproject in superprojectsToAlign)
        {
            string submoduleWorkdir = superproject.Workdir + @$"\{submoduleToAlign}";

            foreach (string branchToAlign in superproject.branchesToAlign)
            {
               
                // checkout branch in superproject
                GitFacade.Switch(superproject.Workdir, branchToAlign);
                GitFacade.FetchAndPull(superproject.Workdir);

                // checkout and pull submodule branch
                GitFacade.Switch(submoduleWorkdir, branchToAlign); // checkout
                GitFacade.FetchAndPull(submoduleWorkdir); // fetch and pull

                // Forward submodule in superproject
                GitFacade.AddAndCommit(superproject.Workdir, submoduleToAlign);

                if(/*SAFE MODE DISABLED*/true)
                {
                    GitFacade.Push(superproject.Workdir);
                }
            }
        }
    }

    private record AligningSuperproject(string Workdir, List<string> branchesToAlign);
}