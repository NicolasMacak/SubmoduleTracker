// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using LibGit2Sharp;

Console.WriteLine("Hello, World!");

// After merge check
// submodul ukazuje spravne? commit na ktory ukazuje sa nachadza v KOLEKCII COMMITOV ORIGIN BRANCHE
 
// Check zarovnania submodelu
// fetch
// poslednu verziu submodulu na dev/test a skontroluje, ci na nu ukazuju vsetky superprojekty

string repoPath = @"C:\NON_SYSTEM\Superproject-A";

string devBranch = "dev";
string testBranch = "test";

// Ako sa to bude spravat ked nie vsetky commity su pushnute na remove?

//git submodule update --init --recursive

using (var repo= new Repository(repoPath))
{
    // super project
    string superprojectName = repoPath.Split(@"\").Last();
        // Relevant branches


            // Submodules
            // Branch 
            // Target commit 
            // Date



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