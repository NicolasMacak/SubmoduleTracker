// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using LibGit2Sharp;

Console.WriteLine("Hello, World!");

// After merge check
// submodul ukazuje spravne? commit na ktory ukazuje sa nachadza v KOLEKCII COMMITOV ORIGIN BRANCHE
 
// Check zarovnania submodelu
// fetch
// poslednu verziu submodulu na dev/test a skontroluje, ci na nu ukazuju vsetky superprojekty

string repoPath = @"C:\NON_SYSTEM\Projects\ActivityRegistrator.BE";
List<string> observedBranches = new() { "adas",  };


// Ako sa to bude spravat ked nie vsetky commity su pushnute na remove?

using (var repo= new Repository(repoPath))
{

}


// Git  fetch
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