using SubmoduleTracker.Core.CommonTypes.SuperProjects;
using SubmoduleTracker.Core.ConsoleTools;
using SubmoduleTracker.Core.ConsoleTools.Constants;
using SubmoduleTracker.Core.GitInteraction.Extensions;
using SubmoduleTracker.Core.GitInteraction.Model;
using static SubmoduleTracker.Core.SubmoduleAlignmentTable.DynamicTableColumn;
using static SubmoduleTracker.Core.SubmoduleAlignmentTable.TableConstants;

namespace SubmoduleTracker.Core.SubmoduleAlignmentTable;
public static class SubmoduleAlignmentTablePrinter
{
    public static void PrintTableForSuperProjects(List<RobustSuperProject> allSuperprojects, List<GitBranch> relevantBranches, List<string>? relevantSubmodules = null)
    {
        if (allSuperprojects != null && allSuperprojects.Count == 0)
        {
            throw new ArgumentNullException($"{nameof(allSuperprojects)} is not supposed to be empty!");
        }

        if (relevantBranches != null && relevantBranches.Count == 0)
        {
            throw new ArgumentNullException($"{nameof(relevantBranches)} is not supposed to be empty!");
        }

        Dictionary<string, DynamicTableColumn> dynamicColumnsConfigurations = GetColumnsConfig(allSuperprojects!, relevantBranches!);

        PrintTableHeader(dynamicColumnsConfigurations);

        foreach (RobustSuperProject superProject in allSuperprojects!)
        {
            relevantSubmodules = relevantSubmodules != null
                ? superProject.SubmodulesNames.Where(x => relevantSubmodules!.Contains(x)).ToList()
                : superProject.SubmodulesNames;

            if (relevantSubmodules.Count == 0)
            {
                CustomConsole.WriteErrorLine($"Skipping {superProject.Name}, because it did not contain any requested submodules: {string.Join(",", relevantSubmodules)}");
                continue;
            }

            PrintTableBody(superProject, relevantBranches!, relevantSubmodules, dynamicColumnsConfigurations);
        }
    }

    public static void PrintTableHeader(Dictionary<string, DynamicTableColumn> columns)
    {
        Console.WriteLine();
        Console.WriteLine(
            columns[Column.SuperProject].GetAdjustedTextation(Column.SuperProject, 0) + Delimiter +
            columns[Column.Branch].GetAdjustedTextation(Column.Branch, 0) + Delimiter +
            columns[Column.Submodule].GetAdjustedTextation(Column.Submodule, 0) + Delimiter +
            columns[Column.IndexCommit].GetAdjustedTextation(Column.IndexCommit, 0) + Delimiter +
            columns[Column.HeadCommit].GetAdjustedTextation(Column.HeadCommit, 0) + Delimiter
        );
    }

    private static void PrintTableBody(RobustSuperProject relevantSuperProject, List<GitBranch> relevantBranches, List<string> relevantSubmodules, Dictionary<string, DynamicTableColumn> columns)
    {
        // Superproject row
        CustomConsole.WriteLineColored(columns[Column.SuperProject].GetAdjustedTextation(relevantSuperProject.Name, 0), TextType.ImporantText);

        foreach (string branch in relevantBranches.GetRemotes())
        {
            // Outputs bellow are in one row. We put offset only into the first
            int offsetForBranchColumn = columns[Column.SuperProject].Width;
            Console.WriteLine(columns[Column.Branch].GetAdjustedTextation(branch, offsetForBranchColumn + Delimiter.Length) + Delimiter);

            // Index comparison row
            foreach (string submoduleName in relevantSubmodules)
            {
                Console.Write(columns[Column.Submodule].GetAdjustedTextation(submoduleName, columns[Column.SuperProject].Width + columns[Column.Branch].Width + Delimiter.Length * 2) + Delimiter);

                string indexCommit = relevantSuperProject.IndexCommitRefs[branch][submoduleName]; // where submodule points on in this branch. .First() because 
                string headCommit = relevantSuperProject.HeadCommitRefs[branch][submoduleName]; // HEAD commit on this branch

                ConsoleColor color = indexCommit == headCommit ? ConsoleColor.Green : ConsoleColor.Red; // aligned on branch? green : red
                string indexCommitFormatted = columns[Column.IndexCommit].GetAdjustedTextation(indexCommit, 0); // formatted value
                CustomConsole.WriteColored(indexCommitFormatted, color);
                Console.Write(Delimiter); // appending delimeter in normal color

                Console.Write(columns[Column.HeadCommit].GetAdjustedTextation(headCommit, 0) + Delimiter + Environment.NewLine);
            }
        }

        Console.WriteLine();
    }

    private static Dictionary<string, DynamicTableColumn> GetColumnsConfig(List<RobustSuperProject> superProjects, List<GitBranch> relevantBranches)
    {
        return new Dictionary<string, DynamicTableColumn>()
        {
            // Superproject
            {
                Column.SuperProject,
                new DynamicTableColumn(
                    width: CalculateColumnWidth(
                                headerLength: Column.SuperProject.Length,
                                longestValueLength: superProjects.MaxBy(x => x.Name)!.Name.Length
                        )
                    )
            },

            // branch
            {
                Column.Branch,
                new DynamicTableColumn(
                        width: CalculateColumnWidth(
                                headerLength: Column.Branch.Length,
                                longestValueLength: relevantBranches.MaxBy(x => x.RemoteName.Length)!.RemoteName.Length
                        )
                )
            },

            // Subomdule
            {
                Column.Submodule,
                new DynamicTableColumn(
                    width: CalculateColumnWidth(
                        headerLength: Column.Submodule.Length,
                        longestValueLength: superProjects.SelectMany(x => x.SubmodulesNames).MaxBy(x => x.Length)!.Length
                    )
                )
            },

            // Index commit
            {
                Column.IndexCommit,
                new DynamicTableColumn(MaxColumnWidth)
            },

            // Head commit
            {
                Column.HeadCommit,
                new DynamicTableColumn(MaxColumnWidth)
            }
        };
    }
}