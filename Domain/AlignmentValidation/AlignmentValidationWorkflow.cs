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
        string selectedSuperproject = getSelectedSuperprojectName();

        IEnumerable<MetaSuperProject> metaSuperprojectValidate = _userConfigfacade.MetaSupeprojects;

        if (selectedSuperproject != AllSuperprojects)
        {
            metaSuperprojectValidate = metaSuperprojectValidate.Where(x => x.Name == selectedSuperproject);
        }

        foreach (MetaSuperProject metaSuperProject in metaSuperprojectValidate)
        {
            RobustSuperProject robustSuperProject = metaSuperProject.ToRobustSuperproject(_userConfigfacade.RelevantBranches);
            CommitsIndexValidationTablePrinter.PrintTable(robustSuperProject);
        }

    }

    private string getSelectedSuperprojectName()
    {
        List<string> superprojectOptions = _userConfigfacade.MetaSupeprojects
            .Select(x  => x.Name)
            .ToList();

        superprojectOptions.Add(AllSuperprojects);

        int? selectedSuperprojectIndex = CustomConsole.GetIndexFromChoices(superprojectOptions, "Zvolte superprojekt ktory chcete zvalidovat.", "Zadajte \"\" pre navrat do hlavneho menu");
        if (!selectedSuperprojectIndex.HasValue)
        {
            _navigationService.Navigate(typeof(HomeScreenWorkflow));
        }

        return selectedSuperprojectIndex == superprojectOptions.Count - 1
            ? AllSuperprojects
            : superprojectOptions[selectedSuperprojectIndex!.Value];
    }
}