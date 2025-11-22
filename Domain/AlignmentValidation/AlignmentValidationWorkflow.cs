using SubmoduleTracker.Core.ConsoleTools;
using SubmoduleTracker.Core.GitInteraction.Model;
using SubmoduleTracker.Domain.Navigation;
using SubmoduleTracker.Domain.UserSettings.Services;

namespace SubmoduleTracker.Domain.AlignmentValidation;
public sealed class AlignmentValidationWorkflow : IWorkflow
{
    private const string AllSuperprojects = "All Superprojects";
    private readonly UserConfigFacade _userConfigfacade;

    public AlignmentValidationWorkflow(UserConfigFacade  userConfigFacade)
    {
        _userConfigfacade = userConfigFacade;
    }

    public void Run()
    {
        Console.Clear();
        string selectedSuperproject = SelectRange();

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

    private string SelectRange()
    {
        List<string> superprojectOptions = _userConfigfacade.MetaSupeprojects
            .Select(x  => x.Name)
            .ToList();

        superprojectOptions.Add(AllSuperprojects);

        CustomConsole.WriteLineColored("Select superproject", ConsoleColor.DarkCyan);

        for (int i = 0; i < superprojectOptions.Count; i++)
        {
            Console.WriteLine($"{i}. {superprojectOptions[i]}");
        }

        string? choice = Console.ReadLine();

        if (string.IsNullOrEmpty(choice))
        {
            // jozef vajda. vrat sa naspat na home screen
        }

        int? userChoice = ConsoleValidation.ReturnValidatedNumberOption(choice, superprojectOptions.Count);

        // invalid
        if(!userChoice.HasValue)
        {
            CustomConsole.WriteErrorLine("Select the number from the provided range");
            SelectRange();
        }

        return userChoice == superprojectOptions.Count - 1
            ? AllSuperprojects
            : superprojectOptions[userChoice!.Value];
    }
}