using System.Xml.Serialization;
using SubmoduleTracker.ConsoleTools;
using SubmoduleTracker.GitInteraction.CLI;
using SubmoduleTracker.GitInteraction.Model;
using SubmoduleTracker.UserSettings.Model;

namespace SubmoduleTracker.SubmoduleAlignment;
public static class SubmoduleAlignment
{
    public static void Index(UserConfig userConfig)
    {
        List<SuperProject> allSuperprojcts = GetAllSuperprojects(userConfig.SuperProjects);

        // Superprojects that contain submodule(selected by user in this method)
        string selectedSubmodule = LetUserSelectSubmodule(allSuperprojcts);
        List<SuperProject> relevantSuperprojects = allSuperprojcts.Where(x => x.SubmodulesNames.Contains(selectedSubmodule)).ToList();

        PrintTable();
        List<AligningSuperproject> superprojectsToAlign = GetSuperProjectsToAlign(selectedSubmodule, relevantSuperprojects);

        // Begin alignemnt process
        AlignSuperprojects(selectedSubmodule, superprojectsToAlign);

        // Zarovnat v Superprojektoch x, y, z?

        // Safe mode

        // Handholding
    }

    private static List<SuperProject> GetAllSuperprojects(List<SuperProjectConfig> superProjectConfigs)
    {
        List<SuperProject> allSuperprojects = new();

        foreach(SuperProjectConfig superProjectConfig in superProjectConfigs)
        {
            SuperProject superproject = new(superProjectConfig.WorkingDirectory);
            allSuperprojects.Add(superproject);
        }

        return allSuperprojects;
    }

    /// <summary>
    /// Submodules from all superprojects are listed. User picks one. Superprojets that contain this submodule are returned
    /// </summary>
    /// <returns>Superprojects that contain submodule selected by user</returns>
    private static string LetUserSelectSubmodule(List<SuperProject> allSuperprojects, string? errorMessage = null)
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

    private static void PrintTable()
    {
        // relevant branch
        // superproject
        // index
        // HEAD
    }

    private static List<AligningSuperproject> GetSuperProjectsToAlign(string selectedSubmodule, List<SuperProject> relevantSuperprojects)
    {
        List<AligningSuperproject> superProjectsToAlign = new();

        List<string> relevantBranches = new () { "dev", /*"test"*/ };

        foreach (SuperProject superProject in relevantSuperprojects)
        {
            // Data on which we make comparison
            // Information where submodule of this superprojects points on provided branches
            Dictionary<string, Dictionary<string, string>> pointings = superProject.GetSubmoduleIndexCommitsRefs(relevantBranches, new List<string> { selectedSubmodule });
            Dictionary<string, Dictionary<string, string>> heads = superProject.GetSubmoduleHeadCommitRefs(relevantBranches, new List<string> { selectedSubmodule });

            List<string> branchesToAlign = new();

            foreach(string branch in relevantBranches)
            {
                string indexCommit = pointings[branch].First().Value; // where submodule points on in this branch
                string headCommit = heads[branch].First().Value; // HEAD commit on this branch

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