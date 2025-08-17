// See https://aka.ms/new-console-template for more information
using SubmoduleTracker.SubmoduleIndexValidation;
using SubmoduleTracker.GitInteraction.Model;
using SubmoduleTracker.SubmoduleIndexValidation.Dto;

// Config file
/*

Superprojects

budu mat full path, ale ich meno bude inou farbou. No submodules check
ak budu priecinky zmazane, vypisu sa cervenou

tracked branches. Pridat/Ignore/Delete

Settings

 */

string repoPath = @"C:\NON_SYSTEM\Superproject-A";

const string SuperProjectName = "Superproject-A";

List<string> relevantBranches = new() { "test", "dev" };

SuperProject superProject = new(repoPath);

PrintableSuperprojectDto printableSuperprojectDto = new()
{
    Title = superProject.Name,
    RevelantBranches = relevantBranches,
    Submodules = superProject .SubmodulesNames,
    SubmoduleCommitIndexes = await superProject.GetSubmoduleIndexCommitsRefs(relevantBranches),
    SubmodulesHeadCommits = await superProject.GetSubmoduleHeadCommitRefs(relevantBranches),
};

CommitsIndexValidationTable.GenerateOutput(printableSuperprojectDto);

return 0;
