using SubmoduleTracker.Core.ConsoleTools;
using SubmoduleTracker.Core.GitInteraction.Model;
using SubmoduleTracker.Core.TablePrinting;
using static SubmoduleTracker.Core.TablePrinting.TableConstants;

namespace SubmoduleTracker.Domain.AlignmentValidation;

/// <summary>
/// Designed to print output of a specific kidn
/// </summary>
public static class CommitsIndexValidationTablePrinter
{
    public static void PrintTable(RobustSuperProject superProject)
    {
        Dictionary<string, DynamicTableColumn> columns = GetConfiguredDynamicTableColumns(superProject);

        PrintTableHeader(columns);

        PrintTableBody(superProject, columns);
    }

    /// <summary>
    /// Prints table that informs if submodules in superproject points to the headcommits in submodules
    /// </summary>
    private static void PrintTableBody(RobustSuperProject superProject, Dictionary<string, DynamicTableColumn> columns)
    {

        // Superproject title
        Console.WriteLine(columns[Column.SuperProject].GetAdjustedTextation(superProject.Name + "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", 0));

        foreach (string branch in superProject.GetAvailableBranches())
        {
            // Branch title
            Console.WriteLine(columns[Column.Branch].GetAdjustedTextation(branch, columns[Column.SuperProject].Width + Delimiter.Length));

            foreach (string submoduleName in superProject.SubmodulesNames)
            {
                Dictionary<string, string> superprojectIndexes = superProject.IndexCommitRefs[branch];
                Dictionary<string, string> submoduleHeadCommits = superProject.HeadCommitRefs[branch];

                // Submodule 
                Console.Write(columns[Column.Submodule].GetAdjustedTextation(submoduleName, columns[Column.SuperProject].Width + columns[Column.Branch].Width + Delimiter.Length * 2) + Delimiter);

                // Index commit
                // Does submodule points correctly?
                Console.ForegroundColor
                    = superprojectIndexes[submoduleName] == submoduleHeadCommits[submoduleName]
                        ? ConsoleColor.Green 
                        : ConsoleColor.Red;

                Console.Write(superprojectIndexes[submoduleName]);

                Console.ForegroundColor = ConsoleColor.White;

                // Head commit
                Console.WriteLine(Delimiter + submoduleHeadCommits[submoduleName]);
            }
        }
    }

    /// <summary>
    /// Columns adjusted for printing table where columns have dynamic width
    /// </summary>
    /// <remarks>
    /// Non-commit columns have dynamic width
    /// </remarks>
    private static Dictionary<string, DynamicTableColumn> GetConfiguredDynamicTableColumns(RobustSuperProject printableSuperprojectDto)
    {
        return new()
        {
            // Superproject
            {
                Column.SuperProject,
                new DynamicTableColumn(
                    DynamicTableColumn.CalculateColumnWidth(
                        columnHeader: Column.SuperProject,
                        longestValueLength: printableSuperprojectDto.Name.Length))
            },

            // Branch
            {
                Column.Branch, 
                new DynamicTableColumn(
                DynamicTableColumn.CalculateColumnWidth(
                        columnHeader: Column.Branch,
                        longestValueLength: printableSuperprojectDto.GetAvailableBranches().MaxBy(x => x.Length)!.Length))
            },

            // Submodule
            {
                Column.Submodule,
                new DynamicTableColumn(
                    DynamicTableColumn.CalculateColumnWidth(
                        columnHeader: Column.Submodule,
                        longestValueLength: printableSuperprojectDto.SubmodulesNames.MaxBy(x => x.Length)!.Length))
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
            },
        };
    }

    private static void PrintTableHeader(Dictionary<string, DynamicTableColumn> dynamicTableColumns) {
        Console.WriteLine();

        CustomConsole.WriteLineColored(
            dynamicTableColumns[Column.SuperProject].GetAdjustedTextation(Column.SuperProject, 0) + Delimiter + 
            dynamicTableColumns[Column.Branch].GetAdjustedTextation(Column.Branch, 0) + Delimiter + 
            dynamicTableColumns[Column.Submodule].GetAdjustedTextation(Column.Submodule, 0) + Delimiter  + 
            dynamicTableColumns[Column.IndexCommit].GetAdjustedTextation(Column.IndexCommit, 0) + Delimiter  + 
            dynamicTableColumns[Column.HeadCommit].GetAdjustedTextation(Column.HeadCommit, 0) + Delimiter  + Environment.NewLine,
            ConsoleColor.DarkCyan
        );
    }
}