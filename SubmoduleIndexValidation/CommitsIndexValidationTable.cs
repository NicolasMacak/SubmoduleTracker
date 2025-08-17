using SubmoduleTracker.SubmoduleIndexValidation.Dto;
using static SubmoduleTracker.Core.TableConstants;

namespace SubmoduleTracker.SubmoduleIndexValidation;

/// <summary>
/// Designed to print output of a specific kidn
/// </summary>
public static class CommitsIndexValidationTable
{
    private const string Delimeter = " | ";

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
        Console.WriteLine(columns[Column.SuperProject].GetPrintableValue(printableSuperprojectDto.Title, 0));

        foreach (string branch in printableSuperprojectDto.RevelantBranches)
        {
            // Branch title
            Console.WriteLine(columns[Column.Branch].GetPrintableValue(branch, columns[Column.SuperProject].Width + Delimeter.Length));

            foreach (string submoduleName in printableSuperprojectDto.Submodules)
            {
                Dictionary<string, string> superprojectIndexes = printableSuperprojectDto.SubmoduleCommitIndexes[branch];
                Dictionary<string, string> submoduleHeadCommits = printableSuperprojectDto.SubmodulesHeadCommits[branch];

                // Submodule 
                Console.Write(columns[Column.Submodule].GetPrintableValue(submoduleName, columns[Column.SuperProject].Width + columns[Column.Branch].Width + Delimeter.Length * 2) + Delimeter);

                // Index commit
                Console.ForegroundColor = superprojectIndexes[submoduleName] == submoduleHeadCommits[submoduleName] ? // Does submodule points correctly?
                    ConsoleColor.Green
                    : ConsoleColor.Red;

                Console.Write(superprojectIndexes[submoduleName]);

                Console.ForegroundColor = ConsoleColor.White;

                // Head commit
                Console.WriteLine(Delimeter + submoduleHeadCommits[submoduleName]);
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
            columns[Column.SuperProject].GetPrintableHeader() + Delimeter + 
            columns[Column.Branch].GetPrintableHeader() + Delimeter + 
            columns[Column.Submodule].GetPrintableHeader() + Delimeter + 
            columns[Column.IndexCommit].GetPrintableHeader() + Delimeter + 
            columns[Column.HeadCommit].GetPrintableHeader() + Delimeter + Environment.NewLine
        );
    }
}