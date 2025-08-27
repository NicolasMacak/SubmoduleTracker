using SubmoduleTracker.GitInteraction.Model;
using SubmoduleTracker.SubmoduleIndexValidation;
using SubmoduleTracker.SubmoduleIndexValidation.Dto;
using SubmoduleTracker.UserSettings;
using SubmoduleTracker.UserSettings.Model;

namespace SubmoduleTracker.HomeScreen;


/*
 
Nacitanie superprojektu nacita submoduly

Submoduly budu taktiez evidovane

listing to bude urco. a mejby farby.

co krok, to save

*/
public static class HomeScreenService
{
    public static void ShowHomeScreen()
    {
        // get config file
        UserConfig config = UserSettingsScreen.GetUserConfiguration();

        // Superprojects
        UserSettingsScreen.Main(config);

        // Submodules
    }

    public static void ShowMainOptions()
    {
        Console.WriteLine("1. Zmenit nastavenia");
        Console.WriteLine("2. Porovnat submoduly");
    }

    public static async void GenerateReport(string superProjectPath)
    {
        SuperProject superProject = new(superProjectPath);

        // Zatial budu iba hlavne branche iba v options.
        List<string> relevantBranches = new() { "test", "dev" };

        PrintableSuperprojectDto printableSuperprojectDto = new()
        {
            Title = superProject.Name,
            RevelantBranches = relevantBranches,
            Submodules = superProject.SubmodulesNames,
            SubmoduleCommitIndexes = await superProject.GetSubmoduleIndexCommitsRefs(relevantBranches),
            SubmodulesHeadCommits = await superProject.GetSubmoduleHeadCommitRefs(relevantBranches),
        };

        CommitsIndexValidationTable.GenerateOutput(printableSuperprojectDto);
    }
}

//string repoPath = @"C:\NON_SYSTEM\Superproject-A";

//    const string SuperProjectName = "Superproject-A";

//    List<string> relevantBranches = new() { "test", "dev" };

//    SuperProject superProject = new(repoPath);

//    PrintableSuperprojectDto printableSuperprojectDto = new()
//    {
//        Title = superProject.Name,
//        RevelantBranches = relevantBranches,
//        Submodules = superProject.SubmodulesNames,
//        SubmoduleCommitIndexes = await superProject.GetSubmoduleIndexCommitsRefs(relevantBranches),
//        SubmodulesHeadCommits = await superProject.GetSubmoduleHeadCommitRefs(relevantBranches),
//    };

//CommitsIndexValidationTable.GenerateOutput(printableSuperprojectDto);
