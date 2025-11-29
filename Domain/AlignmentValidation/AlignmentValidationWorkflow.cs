using SubmoduleTracker.Core.ConsoleTools;
using SubmoduleTracker.Core.GitInteraction.Model;
using SubmoduleTracker.Domain.Navigation;
using SubmoduleTracker.Domain.UserSettings.Services;

namespace SubmoduleTracker.Domain.AlignmentValidation;
public sealed class AlignmentValidationWorkflow : IWorkflow
{
    private const string AllSuperprojects = "All Superprojects";

    private readonly UserConfigFacade _userConfigfacade;
    private readonly NavigationService _navigationService;

    public AlignmentValidationWorkflow(UserConfigFacade  userConfigFacade, NavigationService navigationService)
    {
        _userConfigfacade = userConfigFacade;
        _navigationService = navigationService;
    }

    public void Run()
    {
        Console.Clear();

        List<string> allSelectSuperprojectOptions = _userConfigfacade.MetaSupeprojects.Select(x => x.Name).ToList();
        // If There is more than 1 superproject we want to provide "all superproject" options
        if (allSelectSuperprojectOptions.Count > 1)
        {
            allSelectSuperprojectOptions.Add(AllSuperprojects);
        }

        int? selectedSuperprojectIndex = CustomConsole.GetIndexFromChoices(allSelectSuperprojectOptions, "Zvolte superprojekt ktory chcete zvalidovat.", "Zadajte \"\" pre navrat do hlavneho menu");

        if (!selectedSuperprojectIndex.HasValue)
        {
            _navigationService.Navigate(typeof(HomeScreenWorkflow));
            return;
        }

        IEnumerable<MetaSuperProject> metaSuperprojectValidate = selectedSuperprojectIndex == allSelectSuperprojectOptions.Count - 1
            ? _userConfigfacade.MetaSupeprojects // User selected All superprojects options
            : _userConfigfacade.MetaSupeprojects.Where(x => x.Name == allSelectSuperprojectOptions[selectedSuperprojectIndex.Value]); // User selected specific 

        foreach (MetaSuperProject metaSuperProject in metaSuperprojectValidate)
        {
            RobustSuperProject robustSuperProject = metaSuperProject.ToRobustSuperproject(_userConfigfacade.RelevantBranches);
            CommitsIndexValidationTablePrinter.PrintTable(robustSuperProject);
        }
    }
}