using SubmoduleTracker.Dto;

using static SubmoduleTracker.Core.TableConstants;

namespace SubmoduleTracker.ConsoleOutput;

/// <summary>
/// Designed to print output of a specific kidn
/// </summary>
public static class SubmoduleMergeReport
{
    private const string Delimeter = " | ";

    public static void GenerateOutput(PrintableSuperprojectDto printableSuperproject)
    {
        Dictionary<string, TableColumn> columns = GetColumns(printableSuperproject);

        PrintTableHeader(columns);

        PrintTableBody(printableSuperproject, columns);
    }

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

    private static bool DoesSubmodulePointsCorrectly(string indexCommit, string headCommit)
    {
        return indexCommit == headCommit;
    }
}