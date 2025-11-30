using SubmoduleTracker.Core.ConsoleTools;
using SubmoduleTracker.Domain.AlignmentExecution;
using SubmoduleTracker.Domain.AlignmentValidation;
using SubmoduleTracker.Domain.Navigation;
using SubmoduleTracker.Domain.UserSettings;

namespace SubmoduleTracker.Domain.HomeScreen;

public class HomeScreenWorkflow : IWorkflow
{
    private readonly NavigationService _navigationService;

    public HomeScreenWorkflow(NavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    public void Run()
    {
        Console.Clear();
        CustomConsole.WriteLineColored("Hlavne menu", TextType.ImporantText);
        List<string> choices = new() { "Validacia zarovnania", "Zarovnanie submodulov", "Nastavenia" };

        int? choice = CustomConsole.GetIndexOfUserChoice(choices, "Vyberte akciu");

        if (!choice.HasValue)
        {
            return;
        }

        switch (choice!.Value)
        {
            case 0: _navigationService.Navigate<AlignmentValidationWorkflow>(); break;
            case 1: _navigationService.Navigate<AlignmentExecutionWorkflow>(); break;
            case 2: _navigationService.Navigate<ManageUserSettingsWorkflow>(); break;
        }
    }
}
