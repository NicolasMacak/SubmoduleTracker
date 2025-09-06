// See https://aka.ms/new-console-template for more information
using SubmoduleTracker.SubmoduleIndexValidation;
using SubmoduleTracker.GitInteraction.Model;
using SubmoduleTracker.SubmoduleIndexValidation.Dto;
using SubmoduleTracker.HomeScreen;
using SubmoduleTracker.UserSettings;
using SubmoduleTracker.SubmoduleAlignment;

SubmoduleTracker.UserSettings.Model.UserConfig userConfig = UserSettingsScreen.GetUserConfiguration();

SubmoduleAlignment.Index(userConfig);

return 0;
