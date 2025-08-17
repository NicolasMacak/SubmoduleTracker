using SubmoduleTracker.Core;

namespace SubmoduleTracker.SubmoduleIndexValidation;
public sealed class TableColumn
{
    public string Name { get; }

    public int Width { get; }

    public TableColumn(string name, int width)
    {
        Name = name;
        Width = width;
    }

    /// <summary>
    /// Returns minimal length for every value of the row to fit int
    /// </summary>
    /// <param name="columnName"></param>
    /// <param name="longestValueLength">Column needs to be as long, as it's longest(string length wise) value</param>
    public static int CalculateColumnWidth(string columnName, int longestValueLength)
    {
        int min = Math.Min(longestValueLength, TableConstants.MaxColumnWidth); // as less as required

        return Math.Max(min, columnName.Length); // but more than length of the column name
    }

    public string GetPrintableHeader()
    {
        return Name.PadRight(Width);
    }

    public string GetPrintableValue(string value, int leftOffest)
    {
        return $"{new string(' ', leftOffest)}{value.PadRight(Width)}"
        ;
    }
}