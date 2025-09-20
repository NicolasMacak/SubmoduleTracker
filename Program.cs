// See https://aka.ms/new-console-template for more information
using SubmoduleTracker.SubmoduleIndexValidation;
using SubmoduleTracker.GitInteraction.Model;
using SubmoduleTracker.SubmoduleIndexValidation.Dto;
using SubmoduleTracker.HomeScreen;
using SubmoduleTracker.UserSettings;
using SubmoduleTracker.SubmoduleAlignment;
using SubmoduleTracker.GitInteraction.CLI;

//var aaa = GitFacade.GetCurrentBranch("C:\\NON_SYSTEM");
GitFacade.Switch("C:\\NON_SYSTEM\\Submodule-C", "dev");

SubmoduleTracker.UserSettings.Model.UserConfig userConfig = UserSettingsScreen.GetUserConfiguration();

SubmoduleAlignment.Index(userConfig);

return 0;
