// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using LibGit2Sharp;
using SubmoduleTracker.CLI;
using SubmoduleTracker.Core;
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
string[] relevantBranchesNames = ["dev", "test"];

SuperProject superProject = new SuperProject(repoPath, relevantBranchesNames);

//later superProject.Path
await GitCLI.Checkout(@"C:\NON_SYSTEM\Superproject-A", BranchNames.TEST);
superProject.AddSubmoduleCommitMapForBranch(BranchNames.TEST);

await GitCLI.Checkout(@"C:\NON_SYSTEM\Superproject-A", BranchNames.DEV);
superProject.AddSubmoduleCommitMapForBranch(BranchNames.DEV);

// After merge check
// submodul ukazuje spravne? commit na ktory ukazuje sa nachadza v KOLEKCII COMMITOV ORIGIN BRANCHE

// Check zarovnania submodelu NA REMOTE BRANCHAS. skuska spravnosti. zaujima ma cloud stav

// fetch
// poslednu verziu submodulu na dev/test a skontroluje, ci na nu ukazuju vsetky superprojekty
