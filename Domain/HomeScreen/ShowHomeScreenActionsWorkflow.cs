using SubmoduleTracker.Core.GitInteraction.Model;
using SubmoduleTracker.Domain.SubmoduleIndexValidation;
using SubmoduleTracker.Domain.SubmoduleIndexValidation.Dto;

namespace SubmoduleTracker.Domain.HomeScreen;


/*
 
Nacitanie superprojektu nacita submoduly

Submoduly budu taktiez evidovane

listing to bude urco. a mejby farby.

co krok, to save

*/
public class ShowHomeScreenActionsWorkflow
{
    public void ShowHomeScreen()
    {
        // get config file

        // Superprojects
        //ManageUserSettingsWorkflow.Run();

        // Submodules
    }

    public static void ShowMainOptions()
    {
        Console.WriteLine("1. Zmenit nastavenia");
        Console.WriteLine("2. Porovnat submoduly");
    }

    public static void GenerateReport(string superProjectPath)
    {
        MetaSuperProject superProject = new(superProjectPath);

        // Zatial budu iba hlavne branche iba v options.
        List<string> relevantBranches = new() { "test", "dev" };

        PrintableSuperprojectDto printableSuperprojectDto = new()
        {
            Title = superProject.Name,
            RevelantBranches = relevantBranches,
            Submodules = superProject.SubmodulesNames,
            SubmoduleCommitIndexes = superProject.GetSubmoduleIndexCommitsRefs(relevantBranches, superProject.SubmodulesNames),
            SubmodulesHeadCommits = superProject.GetSubmoduleHeadCommitRefs(relevantBranches, superProject.SubmodulesNames),
        };

        CommitsIndexValidationTable.GenerateOutput(printableSuperprojectDto);
    }
}
