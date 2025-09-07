using SubmoduleTracker.SubmoduleIndexValidation.Dto;
using SubmoduleTracker.TablePrinting;
using static SubmoduleTracker.TablePrinting.TableConstants;

namespace SubmoduleTracker.SubmoduleIndexValidation;

/// <summary>
/// Designed to print output of a specific kidn
/// </summary>
public static class CommitsIndexValidationTable
{
    public static void GenerateOutput(PrintableSuperprojectDto printableSuperproject)
    {
        Dictionary<string, TableColumn> columns = GetColumns(printableSuperproject);

        PrintTableHeader(columns);

        PrintTableBody(printableSuperproject, columns);
    }

    /// <summary>
    /// Prints table that informs if submodules in superproject points to the headcommits in submodules
    /// </summary>
    private static void PrintTableBody(PrintableSuperprojectDto printableSuperprojectDto, Dictionary<string, TableColumn> columns)
    {
        // Superproject title
        Console.WriteLine(columns[Column.SuperProject].GetColumnWidthAdjustedValue(printableSuperprojectDto.Title, 0));

        foreach (string branch in printableSuperprojectDto.RevelantBranches)
        {
            // Branch title
            Console.WriteLine(columns[Column.Branch].GetColumnWidthAdjustedValue(branch, columns[Column.SuperProject].Width + Delimiter.Length));

            foreach (string submoduleName in printableSuperprojectDto.Submodules)
            {
                Dictionary<string, string> superprojectIndexes = printableSuperprojectDto.SubmoduleCommitIndexes[branch];
                Dictionary<string, string> submoduleHeadCommits = printableSuperprojectDto.SubmodulesHeadCommits[branch];

                // Submodule 
                Console.Write(columns[Column.Submodule].GetColumnWidthAdjustedValue(submoduleName, columns[Column.SuperProject].Width + columns[Column.Branch].Width + Delimiter.Length * 2) + Delimiter);

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
    private static Dictionary<string, TableColumn> GetColumns(PrintableSuperprojectDto printableSuperprojectDto)
    {
        // Ordered
        return new()
        {
            // Superproject
            {
                Column.SuperProject,
                new TableColumn(Column.SuperProject,
                    TableColumn.CalculateColumnWidth(
                        columnName: Column.SuperProject,
                        longestValueLength: printableSuperprojectDto.Title.Length))
            },

            // Branch
            {
                Column.Branch, 
                new TableColumn(Column.Branch,
                TableColumn.CalculateColumnWidth(
                        columnName: Column.Branch,
                        longestValueLength: printableSuperprojectDto.RevelantBranches.MaxBy(x => x.Length)!.Length))
            },

            // Submodule
            {
                Column.Submodule,
                new TableColumn(Column.Submodule,
                    TableColumn.CalculateColumnWidth(
                        columnName: Column.Submodule,
                        longestValueLength: printableSuperprojectDto.Submodules.MaxBy(x => x.Length)!.Length))
            },

            // Index commit
            {
                Column.IndexCommit,
                new TableColumn(Column.IndexCommit, MaxColumnWidth)
            },

            // Head commit
            {
                Column.HeadCommit,
                new TableColumn(Column.HeadCommit, MaxColumnWidth)
            },
        };
    }

    private static void PrintTableHeader(Dictionary<string, TableColumn> columns) {

        Console.WriteLine(
            columns[Column.SuperProject].GetPrintableHeader() + Delimiter + 
            columns[Column.Branch].GetPrintableHeader() + Delimiter + 
            columns[Column.Submodule].GetPrintableHeader() + Delimiter  + 
            columns[Column.IndexCommit].GetPrintableHeader() + Delimiter  + 
            columns[Column.HeadCommit].GetPrintableHeader() + Delimiter  + Environment.NewLine
        );
    }
}