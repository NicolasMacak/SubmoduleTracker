namespace SubmoduleTracker.Domain.SubmoduleAlignment.Models;
/// <summary>
/// Used when aligning submodule in git repositories
/// </summary>
public sealed class AligningSuperprojectAAA
{
    public AligningSuperprojectAAA(string workingDirectory, List<string> branchesToAlign ) 
    {
        WorkingDirectory = workingDirectory;
        BranchesToAlign = branchesToAlign;
    }

    public string WorkingDirectory;

    public List<string> BranchesToAlign;
}