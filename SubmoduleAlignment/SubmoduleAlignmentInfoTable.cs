using SubmoduleTracker.GitInteraction.Model;
using SubmoduleTracker.SubmoduleIndexValidation.Dto;
using SubmoduleTracker.TablePrinting;
using static SubmoduleTracker.TablePrinting.TableConstants;


namespace SubmoduleTracker.SubmoduleAlignment;
public static class SubmoduleAlignmentInfoTable
{
    public static void PrintTable(string aligningSubmoduleName, List<MetaSuperProject> allSuperprojects, List<string> relevantBranches)
    {
        Dictionary<string, TableColumn> columns = GetColumns(allSuperprojects, relevantBranches);

        PrintTableHeader(columns);
    }

    public static void PrintTableHeader(Dictionary<string, TableColumn> columns)
    {
        Console.WriteLine(
            columns[Column.SuperProject].GetPrintableValue(Column.SuperProject, 0) + Delimiter +
            columns[Column.Branch].GetPrintableValue(Column.Branch, 0) + Delimiter +
            columns[Column.IndexCommit].GetPrintableValue(Column.IndexCommit, 0) + Delimiter +
            columns[Column.HeadCommit].GetPrintableValue(Column.HeadCommit, 0) + Delimiter
        );
    }

    public static void PrintTableBody(string aligningSubmoduleName, List<MetaSuperProject> allSuperprojects, List<string> relevantBranches, Dictionary<string, TableColumn> columns)
    {
        foreach (MetaSuperProject superProject in allSuperprojects)
        {
            //Dictionary<string, Dictionary<string, string>> pointings = data;
            //Dictionary<string, Dictionary<string, string>> heads = data;

            foreach (string branch in relevantBranches)
            {
                Console.WriteLine(columns[Column.SuperProject].GetPrintableValue(superProject.Name, 0));
                // submodule might not be present

                if (!superProject.SubmodulesNames.Contains(aligningSubmoduleName))
                {
                    Console.WriteLine("-");
                    continue;
                }

                Console.Write(columns[Column.Branch].GetPrintableValue(branch, columns[Column.SuperProject].Width) + Delimiter);
            }
        }


    }

    private static Dictionary<string, TableColumn> GetColumns(List<MetaSuperProject> allSuperprojects, List<string> relevantBranches)
    {
        return new Dictionary<string, TableColumn>()
        {
            // Superproject
            {
                TableConstants.Column.SuperProject,
                new TableColumn(
                        string.Empty, // Later removed
                        TableColumn.CalculateColumnWidth(
                                columnName: TableConstants.Column.SuperProject,
                                longestValueLength: allSuperprojects.MaxBy(x => x.Name)!.Name.Length
                        )
                )
            },

            // branch
            {
                TableConstants.Column.SuperProject,
                new TableColumn(
                        string.Empty, // Later removed
                        TableColumn.CalculateColumnWidth(
                                columnName: TableConstants.Column.Branch,
                                longestValueLength: relevantBranches.MaxBy(x => x.Length)!.Length
                        )
                )
            },

            // Index commit
            {
                TableConstants.Column.IndexCommit,
                new TableColumn(string.Empty, // Later removed
                                TableConstants.MaxColumnWidth
                        )
            },

            // Head commit
            {
                TableConstants.Column.HeadCommit,
                new TableColumn(string.Empty, // Later removed
                                TableConstants.MaxColumnWidth
                )
            }
        };
    }
}