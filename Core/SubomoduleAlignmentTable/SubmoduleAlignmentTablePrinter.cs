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
    private const string MissingBranchFillerString = "---";

    /// <summary>
    /// Prints alignment submodule validation table
    /// </summary>
    /// <param name="allSuperprojects"></param>
    /// <param name="relevantBranches"></param>
    /// <param name="relevantSubmodules"></param>
    /// <exception cref="ArgumentNullException"></exception>
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

        Dictionary<string, DynamicTableColumn> dynamicColumnsConfigurations = GetColumnsConfiguration(allSuperprojects!, relevantBranches!);

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

    /// <summary>
    /// Prints table header for the superproject
    /// </summary>
    /// <param name="columns"></param>
    public static void PrintTableHeader(Dictionary<string, DynamicTableColumn> columns)
    {
        Console.WriteLine();
        Console.WriteLine(
            columns[Column.SuperProject].GetHeaderValue(Column.SuperProject) + Delimiter +
            columns[Column.Branch].GetHeaderValue(Column.Branch) + Delimiter +
            columns[Column.Submodule].GetHeaderValue(Column.Submodule) + Delimiter +
            columns[Column.IndexCommit].GetHeaderValue(Column.IndexCommit) + Delimiter +
            columns[Column.HeadCommit].GetHeaderValue(Column.HeadCommit) + Delimiter
        );
    }

    /// <summary>
    /// Prints table body for the superproject
    /// </summary>
    /// <param name="relevantSuperProject"></param>
    /// <param name="relevantBranches"></param>
    /// <param name="relevantSubmodules"></param>
    /// <param name="columns"></param>
    private static void PrintTableBody(RobustSuperProject relevantSuperProject, List<GitBranch> relevantBranches, List<string> relevantSubmodules, Dictionary<string, DynamicTableColumn> columns)
    {
        // Superproject row
        CustomConsole.WriteLineColored(columns[Column.SuperProject].GetHeaderValue(relevantSuperProject.Name), TextType.ImporantText);

        foreach (string branch in relevantBranches.GetRemotes())
        {
            // Outputs bellow are in one row. We put offset only into the first
            //int offsetForBranchColumn = columns[Column.SuperProject]._width;
            Console.WriteLine(columns[Column.Branch].GetBodyValue(branch) + Delimiter);

            // Index comparison row
            foreach (string submoduleName in relevantSubmodules)
            {
                Console.Write(columns[Column.Submodule].GetBodyValue(submoduleName) + Delimiter);

                // where submodule points on in this branch. .First() because 
                string indexCommit = relevantSuperProject.IndexCommitRefs[branch].ContainsKey(submoduleName) // Submodule might not always contain branch. PrintDomain
                    ? relevantSuperProject.IndexCommitRefs[branch][submoduleName]
                    : MissingBranchFillerString;

                // HEAD commit on this branch
                string headCommit = relevantSuperProject.HeadCommitRefs[branch].ContainsKey(submoduleName) // Submodule might not always contain branch. PrintDomain
                    ? relevantSuperProject.HeadCommitRefs[branch][submoduleName]
                    : MissingBranchFillerString;

                ConsoleColor color = ConsoleColor.White;
                if(indexCommit != MissingBranchFillerString)
                {
                    color = indexCommit == headCommit ? ConsoleColor.Green : ConsoleColor.Red; // aligned on branch? green : red
                }
                string indexCommitFormatted = columns[Column.IndexCommit].GetBodyValue(indexCommit); // formatted value
                CustomConsole.WriteColored(indexCommitFormatted, color);
                Console.Write(Delimiter); // appending delimeter in normal color

                Console.Write(columns[Column.HeadCommit].GetBodyValue(headCommit) + Delimiter + Environment.NewLine);
            }
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Configures dynamic columns 
    /// </summary>
    /// <param name="superProjects"></param>
    /// <param name="relevantBranches"></param>
    /// <returns></returns>
    private static Dictionary<string, DynamicTableColumn> GetColumnsConfiguration(List<RobustSuperProject> superProjects, List<GitBranch> relevantBranches)
    {
        int superProjectColumnWidth = CalculateColumnWidth(
                headerLength: Column.SuperProject.Length,
                longestValueLength: superProjects.MaxBy(x => x.Name)!.Name.Length
            );

        int branchColumnWidth = CalculateColumnWidth(
                headerLength: Column.SuperProject.Length,
                longestValueLength: superProjects.MaxBy(x => x.Name)!.Name.Length
            );
        int branchColumnOffset = superProjectColumnWidth + Delimiter.Length;

        int submoduleColumnWidth = CalculateColumnWidth(
                headerLength: Column.Submodule.Length,
                longestValueLength: superProjects.SelectMany(x => x.SubmodulesNames).MaxBy(x => x.Length)!.Length
            );
        int submoduleColumnOffest = superProjectColumnWidth + Delimiter.Length + branchColumnWidth + Delimiter.Length;

        return new Dictionary<string, DynamicTableColumn>()
        {
            // Superproject
            {
                Column.SuperProject,
                new DynamicTableColumn(
                    width: superProjectColumnWidth,
                    leftOffset: 0
                    )
            },

            // branch
            {
                Column.Branch,
                new DynamicTableColumn(
                        width: branchColumnWidth,
                        leftOffset: branchColumnOffset
                        )
            },

            // Subomdule
            {
                Column.Submodule,
                new DynamicTableColumn(
                    width: superProjectColumnWidth,
                    leftOffset: submoduleColumnOffest
                    )
            },

            // Index commit
            {
                Column.IndexCommit,
                new DynamicTableColumn(MaxColumnWidth, 0)
            },

            // Head commit
            {
                Column.HeadCommit,
                new DynamicTableColumn(MaxColumnWidth, 0)
            }
        };
    }
}