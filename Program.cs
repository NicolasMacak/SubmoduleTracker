// See https://aka.ms/new-console-template for more information
using SubmoduleTracker.Core;
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

List<string> relevantBranches = new() { BranchNames.TEST, BranchNames.DEV };

SuperprojectsManager superprojectsManager = new( superProjectsPaths: new List<string> { repoPath }, relevantBranches: relevantBranches);

/// po vykonani tohto na ake commity v submoduloch ukazuju relevantne branche v super projekte
SuperProject superproject = await superprojectsManager.GetSubmodulesIndexCommitsForSuperproject("Superproject-A");

Dictionary<string, string> allSubmodulesWorkids = superprojectsManager.GetSubmodulesWorkdirectories();

SubmodulesManager submoduleManager = new SubmodulesManager(allSubmodulesWorkids);

// po vykonani tohto viem kam smeruju heady submodulov
Dictionary<string, Dictionary<string, string>> headsOfSubmodulesForBranches = await submoduleManager.GetHeadsOfAllSubmodules(relevantBranches);

Console.WriteLine("Superprojects \t Branch \t Submodule \t Index commit \t Head commit");

Console.WriteLine("Superproject A");

Console.WriteLine($"\t {BranchNames.TEST}");


var superprojectPointings = superproject.SubmoduleCommitIndexesForBranches[BranchNames.TEST];
var headCommits = headsOfSubmodulesForBranches[BranchNames.TEST];

foreach(string submoduleName in superproject.SubmodulesNames)
{
    string submoduleIndexCommit = superprojectPointings[submoduleName];
    string submoduleHeadCommit = headCommits[submoduleName];

    Console.WriteLine($"\t \t {submoduleName} \t {submoduleIndexCommit} \t {submoduleHeadCommit}"); // Submodule name / Index commit / Head commit
}

Console.WriteLine("Badam bi mbada bum");

// porovnat to kam ukazuju submoduly
// s tym aky head maju submoduly
// dev.api == api.dev.head

// After merge check
// submodul ukazuje spravne? commit na ktory ukazuje sa nachadza v KOLEKCII COMMITOV ORIGIN BRANCHE

// Check zarovnania submodelu NA REMOTE BRANCHAS. skuska spravnosti. zaujima ma cloud stav

// fetch
// poslednu verziu submodulu na dev/test a skontroluje, ci na nu ukazuju vsetky superprojekty
