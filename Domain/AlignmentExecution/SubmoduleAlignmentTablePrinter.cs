using SubmoduleTracker.Core.ConsoleTools;
using SubmoduleTracker.Core.GitInteraction;
using SubmoduleTracker.Core.GitInteraction.Model;
using SubmoduleTracker.Core.TablePrinting;
using static SubmoduleTracker.Core.TablePrinting.TableConstants;


namespace SubmoduleTracker.Domain.AlignmentExecution;

/// <summary>
/// Prints table 
/// </summary>
public static class SubmoduleAlignmentTablePrinter
{
    public static void PrintTable(string aligningSubmoduleName, List<RobustSuperProject> allSuperprojects, List<GitBranch> relevantBranches)
    {
        Dictionary<string, DynamicTableColumn> columns = GetColumns(allSuperprojects, relevantBranches);

        CustomConsole.WriteColored(Environment.NewLine + $"Aligning Submodule: {aligningSubmoduleName}" + Environment.NewLine, TextType.ImporantText);

        PrintTableHeader(columns);

        PrintTableBody(aligningSubmoduleName, allSuperprojects, relevantBranches, columns);
    }

    public static void PrintTableHeader(Dictionary<string, DynamicTableColumn> columns)
    {
        Console.WriteLine(
            columns[Column.SuperProject].GetAdjustedTextation(Column.SuperProject, 0) + Delimiter +
            columns[Column.Branch].GetAdjustedTextation(Column.Branch, 0) + Delimiter +
            columns[Column.IndexCommit].GetAdjustedTextation(Column.IndexCommit, 0) + Delimiter +
            columns[Column.HeadCommit].GetAdjustedTextation(Column.HeadCommit, 0) + Delimiter
        );
    }

    private static void PrintTableBody(string aligningSubmoduleName, List<RobustSuperProject> relevantSuperProjects, List<GitBranch> relevantBranches, Dictionary<string, DynamicTableColumn> columns)
    {
        foreach (RobustSuperProject superProject in relevantSuperProjects)
        {
            Console.WriteLine(columns[Column.SuperProject].GetAdjustedTextation(superProject.Name + "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", 0));

            foreach (string branch in relevantBranches.GetRemotes())
            {
                int offsetForThisColumn = columns[Column.SuperProject].Width; // TODO. SHOULDNT I USE ALIGNING SUBMODULE NAME?
                string indexCommit = superProject.IndexCommitRefs[branch][aligningSubmoduleName]; // where submodule points on in this branch. .First() because 
                string headCommit = superProject.HeadCommitRefs[branch][aligningSubmoduleName]; // HEAD commit on this branch

                // Outputs bellow are in one row. We put offset only into the first
                Console.Write(columns[Column.Branch].GetAdjustedTextation(branch, offsetForThisColumn + Delimiter.Length) + Delimiter);

                ConsoleColor color = indexCommit == headCommit ? ConsoleColor.Green : ConsoleColor.Red; // aligned on branch? green : red
                string indexCommitFormatted = columns[Column.IndexCommit].GetAdjustedTextation(indexCommit, 0); // formatted value
                CustomConsole.WriteColored(indexCommitFormatted, color);
                Console.Write(Delimiter); // appending delimeter in normal color

                Console.Write(columns[Column.HeadCommit].GetAdjustedTextation(headCommit, 0) + Delimiter + Environment.NewLine);
            }
        }

        Console.WriteLine();
    }

    private static Dictionary<string, DynamicTableColumn> GetColumns(List<RobustSuperProject> allSuperprojects, List<GitBranch> relevantBranches)
    {
        return new Dictionary<string, DynamicTableColumn>()
        {
            // Superproject
            {
                Column.SuperProject,
                new DynamicTableColumn(MaxColumnWidth                     
                        )
            },

            // branch
            {
                Column.Branch,
                new DynamicTableColumn(
                        DynamicTableColumn.CalculateColumnWidth(
                                columnHeader: Column.Branch,
                                longestValueLength: relevantBranches.MaxBy(x => x.RemoteName.Length)!.RemoteName.Length
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