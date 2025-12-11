using SubmoduleTracker.Core.CommonTypes.SuperProjects;
using SubmoduleTracker.Core.ConsoleTools;
using SubmoduleTracker.Core.GitInteraction.Model;
using SubmoduleTracker.Core.Navigation.Services;
using SubmoduleTracker.Core.SubmoduleAlignmentTable;
using SubmoduleTracker.Domain.HomeScreen;
using SubmoduleTracker.Domain.UserSettings.Services;

namespace SubmoduleTracker.Domain.AlignmentValidation;
public sealed class AlignmentValidationWorkflow : INavigable
{
    private const string AllSuperprojects = "All Superprojects";

    private readonly UserConfigService _userConfigfacade;
    private readonly NavigationService _navigationService;
    private readonly List<string> _allSelectSuperprojectOptions;

    private readonly List<List<GitBranch>> _relevantBranchesOptions = new() {
         new () { new GitBranch("dev"), new GitBranch("test") },
         new () { new GitBranch("master") },
         new () { new GitBranch("dev"), new GitBranch("test"), new GitBranch("master") },
    };

    public AlignmentValidationWorkflow(UserConfigService userConfigFacade, NavigationService navigationService)
    {
        _userConfigfacade = userConfigFacade;
        _navigationService = navigationService;
        _allSelectSuperprojectOptions = _userConfigfacade.MetaSuperprojects.Select(x => x.Name).ToList();

        // If There is more than 1 superproject we want to provide "all superproject" options
        if (_allSelectSuperprojectOptions.Count > 1)
        {
            _allSelectSuperprojectOptions.Add(AllSuperprojects);
        }
    }

    public void Run()
    {
        List<MetaSuperProject>? superprojectToValidate = LetUserChooseSuperprojectsToValidate();
        if (superprojectToValidate == null)
        {
            _navigationService.NavigateTo<HomeScreenWorkflow>();
            return;
        }

        List<GitBranch> relevantBranches = LetUserChooseRelevantBranches();

        SubmoduleAlignmentTablePrinter.PrintTableForSuperProjects(superprojectToValidate.Select(x => x.ToRobustSuperproject(relevantBranches)).ToList(), relevantBranches);

        _navigationService.PromptReturnToMainMenu();
    }

    /// <returns>
    /// Superprojects that will be validated. Or null, if user enters empty string
    /// </returns>
    private List<MetaSuperProject>? LetUserChooseSuperprojectsToValidate()
    {
        List<MetaSuperProject> metaSuperProjectsToReturn = new();

        int? indexOfSuperprojectOptions = UserPrompts.GetIndexOfUserChoice(_allSelectSuperprojectOptions, "Choose superproject to validate.", UserPrompts.ReturnToMainMenuPrompt);
        if (!indexOfSuperprojectOptions.HasValue)
        {
            return null;
        }

        if(indexOfSuperprojectOptions.Value == _allSelectSuperprojectOptions.Count - 1)
        {
            // Picked last option. All superprojects
            metaSuperProjectsToReturn.AddRange(_userConfigfacade.MetaSuperprojects);
        }
        else
        {
            // Picked not last option. Specific superproject
            metaSuperProjectsToReturn.Add(_userConfigfacade.MetaSuperprojects[indexOfSuperprojectOptions.Value]);
        }

        return metaSuperProjectsToReturn;
    }

    private List<GitBranch> LetUserChooseRelevantBranches()
    {
        List<string> relevantBranchesOptions = _relevantBranchesOptions.Select(x => string.Join(", ", x.Select(x => x.RemoteName))).ToList();

        int? indexOfRelevantBranchOption = UserPrompts.GetIndexOfUserChoice(relevantBranchesOptions, "Choose the set of branches to validate.");

        // If we wont use emptyStringPrompt UserPrompts.GetIndexOfUserChoice is not supposed to return null
        if (!indexOfRelevantBranchOption.HasValue)
        {
            throw new InvalidOperationException("Index of selected branches branches options was null.");
        }

        return _relevantBranchesOptions[indexOfRelevantBranchOption.Value];
    }
}