// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using SubmoduleTracker.Model;

Console.WriteLine("Hello, World!");

// After merge check
// submodul ukazuje spravne? commit na ktory ukazuje sa nachadza v KOLEKCII COMMITOV ORIGIN BRANCHE
 
// Check zarovnania submodelu NA REMOTE BRANCHAS. skuska spravnosti. zaujima ma cloud stav

// fetch
// poslednu verziu submodulu na dev/test a skontroluje, ci na nu ukazuju vsetky superprojekty

string repoPath = @"C:\NON_SYSTEM\Superproject-A";

string[] relevantBranchesNames = ["dev", "test"];

Superproject superproject = new Superproject(repoPath, relevantBranchesNames);


// Git  fetch
//git submodule update --init --recursive
var psi = new ProcessStartInfo("git", "fetch --all")
{
    WorkingDirectory = repoPath,
    RedirectStandardOutput = true,
    RedirectStandardError = true
};

using var process = Process.Start(psi);
string output = await process.StandardOutput.ReadToEndAsync();
string error = await process.StandardError.ReadToEndAsync();
process.WaitForExit();