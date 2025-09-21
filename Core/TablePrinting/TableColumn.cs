namespace SubmoduleTracker.Core.TablePrinting;

/// <summary>
/// Allows nicely formatted table.
/// </summary>
// nech sa stara iba o width. hodnoty budu podsuvane inde
public sealed class TableColumn
{
    [Obsolete("Pan Macak, dajte toto pls het")]
    public string Name { get; }

    public int Width { get; }

    public TableColumn(string name, int width)
    {
        Name = name; // dat het
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

    [Obsolete("staci GetPrintableValue")]
    public string GetPrintableHeader()
    {
        return Name.PadRight(Width);
    }

    /// <summary>
    /// Returns value + " " up to the end of the column width. Offset is possibility
    /// </summary>
    /// <param name="value">Value to print</param>
    /// <param name="leftOffest"></param>
    /// <remarks>
    /// Formula: " " * offset + value + " " * (Width - value.length)
    /// </remarks>
    public string GetColumnWidthAdjustedValue(string value, int leftOffest)
    {
        return $"{new string(' ', leftOffest)}{value.PadRight(Width)}";
    }
}