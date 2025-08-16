// See https://aka.ms/new-console-template for more information
using SubmoduleTracker.ConsoleOutput;
using SubmoduleTracker.Core;
using SubmoduleTracker.Dto;
using SubmoduleTracker.Managers;
using SubmoduleTracker.Model;

/*

Superproject 
dev
Superproject: : 606e
Submodule C: cec5d
Submodule D: 56ae

test
Superproject: : f058e
Submodule C: 4d55
Submodule D: 56ae
*/

string repoPath = @"C:\NON_SYSTEM\Superproject-A";

const string SuperProjectName = "Superproject-A";

List<string> relevantBranches = new() { BranchNames.TEST, BranchNames.DEV };

SuperprojectsManager superprojectsManager = new( superProjectsPaths: new List<string> { repoPath }, relevantBranches: relevantBranches);

/// po vykonani tohto na ake commity v submoduloch ukazuju relevantne branche v super projekte
SuperProject superproject = await superprojectsManager.GetSubmodulesIndexCommitsForSuperproject("Superproject-A");

Dictionary<string, string> allSubmodulesWorkids = superprojectsManager.GetSubmodulesWorkdirectories();

SubmodulesManager submoduleManager = new SubmodulesManager(allSubmodulesWorkids);

// po vykonani tohto viem kam smeruju heady submodulov
Dictionary<string, Dictionary<string, string>> headsOfSubmodulesForBranches = await submoduleManager.GetHeadsOfAllSubmodules(relevantBranches);

PrintableSuperprojectDto printableSuperprojectDto = new()
{
    Title = SuperProjectName,
    RevelantBranches = relevantBranches,
    Submodules = superproject.SubmodulesNames,
    SubmoduleCommitIndexes = superproject.SubmoduleCommitIndexesForBranches,
    SubmodulesHeadCommits = headsOfSubmodulesForBranches,
};

SubmoduleMergeReport.GenerateOutput(printableSuperprojectDto);


return 0;

// porovnat to kam ukazuju submoduly
// s tym aky head maju submoduly
// dev.api == api.dev.head

// After merge check
// submodul ukazuje spravne? commit na ktory ukazuje sa nachadza v KOLEKCII COMMITOV ORIGIN BRANCHE

// Check zarovnania submodelu NA REMOTE BRANCHAS. skuska spravnosti. zaujima ma cloud stav

// fetch
// poslednu verziu submodulu na dev/test a skontroluje, ci na nu ukazuju vsetky superprojekty
