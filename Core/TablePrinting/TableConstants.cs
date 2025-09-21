namespace SubmoduleTracker.Core.TablePrinting;
public static class TableConstants
{
    public const int MaxColumnWidth  = 20;

    public const string Delimiter = " | ";

    public static class Column
    {
        public const string SuperProject = "Superproject";
        public const string Branch = "Branch";
        public const string Submodule = "Submodule";
        public const string IndexCommit = "Index Commit";
        public const string HeadCommit = "Head Commit";
    }
}