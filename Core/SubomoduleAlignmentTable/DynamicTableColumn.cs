namespace SubmoduleTracker.Core.SubmoduleAlignmentTable;

/// <summary>
/// Used to create columns with flexible width.
/// </summary>
/// <remarks>
/// 
/// </remarks>
public sealed class DynamicTableColumn
{
    public int Width { get; }

    public DynamicTableColumn(int width)
    {
        Width = width;
    }

    /// <summary>
    /// Returns minimal length for every value of the row to fit int
    /// </summary>
    /// <param name="columnHeader"></param>
    /// <param name="longestValueLength">Column needs to be as long, as it's longest(string length wise) value</param>
    public static int CalculateColumnWidth(int headerLength, int longestValueLength)
    {
        int min = Math.Min(longestValueLength, TableConstants.MaxColumnWidth); // as thin as possible. Not more than maximum column of the width

        return Math.Max(min, headerLength); // but at least as wide as header
    }

    /// <summary>
    /// Returns value + " " up to the end of the column width. Offset is possibility
    /// </summary>
    /// <param name="value">Value to print</param>
    /// <param name="leftOffest"></param>
    /// <remarks>
    /// Formula: " " * offset + value + " " * (Width - value.length)
    /// </remarks>
    public string GetAdjustedTextation(string value, int leftOffest)
    {
        if (value.Length > Width)
        {
            value = value[..(Width - 2)] + "..";
        }

        return $"{new string(' ', leftOffest)}{value.PadRight(Width)}";
    }
}