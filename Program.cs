// See https://aka.ms/new-console-template for more information
using SubmoduleTracker.SubmoduleIndexValidation;
using SubmoduleTracker.GitInteraction.Model;
using SubmoduleTracker.SubmoduleIndexValidation.Dto;
using SubmoduleTracker.HomeScreen;

// Config file
/*

Superprojects

budu mat full path, ale ich meno bude inou farbou. No submodules check
ak budu priecinky zmazane, vypisu sa cervenou

tracked branches. Pridat/Ignore/Delete

Settings

 */

HomeScreenService babka = new HomeScreenService();
babka.GetUserConfiguration();

return 0;
