using SubmoduleTracker.Core.ConsoleTools;
using SubmoduleTracker.Core.GitInteraction.Model;
using SubmoduleTracker.Core.TablePrinting;
using static SubmoduleTracker.Core.TablePrinting.TableConstants;


namespace SubmoduleTracker.Domain.AlignmentExecution;

/// <summary>
/// Prints table 
/// </summary>
public static class SubmoduleAlignmentTablePrinter
{
    public static void PrintTable(string aligningSubmoduleName, List<RobustSuperProject> allSuperprojects, List<string> relevantBranches)
    {
        Dictionary<string, TableColumn> columns = GetColumns(allSuperprojects, relevantBranches);

        CustomConsole.WriteColored(Environment.NewLine + $"Aligning Submodule: {aligningSubmoduleName}" + Environment.NewLine, TextType.ImporantText);

        PrintTableHeader(columns);

        PrintTableBody(aligningSubmoduleName, allSuperprojects, relevantBranches, columns);
    }

    public static void PrintTableHeader(Dictionary<string, TableColumn> columns)
    {
        Console.WriteLine(
            columns[Column.SuperProject].GetColumnWidthAdjustedValue(Column.SuperProject, 0) + Delimiter +
            columns[Column.Branch].GetColumnWidthAdjustedValue(Column.Branch, 0) + Delimiter +
            columns[Column.IndexCommit].GetColumnWidthAdjustedValue(Column.IndexCommit, 0) + Delimiter +
            columns[Column.HeadCommit].GetColumnWidthAdjustedValue(Column.HeadCommit, 0) + Delimiter
        );
    }

    private static void PrintTableBody(string aligningSubmoduleName, List<RobustSuperProject> relevantSuperProjects, List<string> relevantBranches, Dictionary<string, TableColumn> columns)
    {
        foreach (RobustSuperProject superProject in relevantSuperProjects)
        {
            Console.WriteLine(columns[Column.SuperProject].GetColumnWidthAdjustedValue(superProject.Name, 0));

            foreach (string branch in relevantBranches)
            {
                int offsetForThisColumn = columns[Column.SuperProject].Width;
                string indexCommit = superProject.IndexCommitRefs[branch].First().Value; // where submodule points on in this branch
                string headCommit = superProject.HeadCommitRefs[branch].First().Value; // HEAD commit on this branch

                // Outputs bellow is one row. We put offset only into the first
                Console.Write(columns[Column.Branch].GetColumnWidthAdjustedValue(branch, offsetForThisColumn + Delimiter.Length) + Delimiter);

                ConsoleColor color = indexCommit == headCommit ? ConsoleColor.Green : ConsoleColor.Red; // aligned on branch? green : red
                string indexCommitFormatted = columns[Column.IndexCommit].GetColumnWidthAdjustedValue(indexCommit, 0); // formatted value
                CustomConsole.WriteColored(indexCommitFormatted, color);
                Console.Write(Delimiter); // appending delimeter in normal color

                Console.Write(columns[Column.HeadCommit].GetColumnWidthAdjustedValue(headCommit, 0) + Delimiter + Environment.NewLine);
            }
        }

        Console.WriteLine();
    }

    private static Dictionary<string, TableColumn> GetColumns(List<RobustSuperProject> allSuperprojects, List<string> relevantBranches)
    {
        return new Dictionary<string, TableColumn>()
        {
            // Superproject
            {
                Column.SuperProject,
                new TableColumn(
                        string.Empty, // Later removed
                        TableColumn.CalculateColumnWidth(
                                columnName: Column.SuperProject,
                                longestValueLength: allSuperprojects.MaxBy(x => x.Name)!.Name.Length
                        )
                )
            },

            // branch
            {
                Column.Branch,
                new TableColumn(
                        string.Empty, // Later removed
                        TableColumn.CalculateColumnWidth(
                                columnName: Column.Branch,
                                longestValueLength: relevantBranches.MaxBy(x => x.Length)!.Length
                        )
                )
            },

            // Index commit
            {
                Column.IndexCommit,
                new TableColumn(string.Empty, // Later removed
                                MaxColumnWidth
                        )
            },

            // Head commit
            {
                Column.HeadCommit,
                new TableColumn(string.Empty, // Later removed
                                MaxColumnWidth
                )
            }
        };
    }
}