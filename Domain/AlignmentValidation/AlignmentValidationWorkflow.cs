using SubmoduleTracker.Core.ConsoleTools;
using SubmoduleTracker.Core.GitInteraction.Model;
using SubmoduleTracker.Domain.HomeScreen;
using SubmoduleTracker.Domain.Navigation;
using SubmoduleTracker.Domain.UserSettings.Services;

namespace SubmoduleTracker.Domain.AlignmentValidation;
public sealed class AlignmentValidationWorkflow : IWorkflow
{
    private const string AllSuperprojects = "All Superprojects";

    private readonly UserConfigFacade _userConfigfacade;
    private readonly NavigationService _navigationService;
    private readonly List<string> _allSelectSuperprojectOptions;

    private readonly List<List<GitBranch>> _relevantBranchesOptions = new() {
         new () { new GitBranch("dev"), new GitBranch("test") },
         new () { new GitBranch("master") },
         new () { new GitBranch("dev"), new GitBranch("test"), new GitBranch("master") },
    };

    public AlignmentValidationWorkflow(UserConfigFacade userConfigFacade, NavigationService navigationService)
    {
        _userConfigfacade = userConfigFacade;
        _navigationService = navigationService;
        _allSelectSuperprojectOptions = _userConfigfacade.MetaSupeprojects.Select(x => x.Name).ToList();
    }

    public void Run()
    {
        Console.Clear();

        int? selectedSuperprojectIndex = LetUserChooseSuperprojects();
        if (!selectedSuperprojectIndex.HasValue)
        {
            _navigationService.Navigate<HomeScreenWorkflow>();
            return;
        }

        IEnumerable<MetaSuperProject> metaSuperprojectValidate = selectedSuperprojectIndex == _allSelectSuperprojectOptions.Count - 1
            ? _userConfigfacade.MetaSupeprojects // User selected All superprojects options
            : _userConfigfacade.MetaSupeprojects.Where(x => x.Name == _allSelectSuperprojectOptions[selectedSuperprojectIndex.Value]); // User selected specific 

        // ---- Branch selection
        List<GitBranch> relevantBranches = LetUserChooseRelevantBranches();

        foreach (MetaSuperProject metaSuperProject in metaSuperprojectValidate)
        {
            RobustSuperProject robustSuperProject = metaSuperProject.ToRobustSuperproject(relevantBranches);
            CommitsIndexValidationTablePrinter.PrintTable(robustSuperProject);
        }
    }

    private int? LetUserChooseSuperprojects()
    {
        // If There is more than 1 superproject we want to provide "all superproject" options
        if (_allSelectSuperprojectOptions.Count > 1)
        {
            _allSelectSuperprojectOptions.Add(AllSuperprojects);
        }

        return CustomConsole.GetIndexOfUserChoice(_allSelectSuperprojectOptions, "Zvolte superprojekt ktory chcete zvalidovat.", "Zadajte \"\" pre navrat do hlavneho menu");
    }

    private List<GitBranch> LetUserChooseRelevantBranches()
    {
        List<string> options = _relevantBranchesOptions.Select(x => string.Join(", ", x.Select(x => x.RemoteName))).ToList();

        int? selectedOptionIndex = CustomConsole.GetIndexOfUserChoice(options, "Zvolte sadu branchov pre validaciu zarovnania.");

        // If we wont use emptyStringPrompt CustomConsole.GetIndexOfUserChoice is not supposed to return null
        if (!selectedOptionIndex.HasValue)
        {
            throw new Exception("Index of selected branches branches options was null.");
        }

        return _relevantBranchesOptions[selectedOptionIndex.Value];
    }
}