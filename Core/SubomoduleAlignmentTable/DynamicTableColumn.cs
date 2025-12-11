namespace SubmoduleTracker.Core.SubmoduleAlignmentTable;

/// <summary>
/// Represents a text-based table column with fixed width and optional left offset.
/// Provides formatting helpers for consistent column alignment.
/// </summary>
public sealed class DynamicTableColumn
{
    private readonly int _width;
    private readonly int _leftOffset;

    /// <param name="width">Total width of the column in characters.</param>
    /// <param name="leftOffset">Number of spaces added before the value.</param>
    public DynamicTableColumn(int width, int  leftOffset)
    {
        _width = width;
        _leftOffset = leftOffset;
    }

    /// <summary>
    /// Calculates the optimal column width based on header length, longest value, and the maximum allowed width.
    /// </summary>
    /// <param name="headerLength">Length of the column header text.</param>
    /// <param name="longestValueLength">Length of the longest value in the column.</param>
    /// <returns>The computed column width.</returns>
    public static int CalculateColumnWidth(int headerLength, int longestValueLength)
    {
        int min = Math.Min(longestValueLength, TableConstants.MaxColumnWidth); // as thin as possible. Not more than maximum column of the width

        return Math.Max(min, headerLength); // but at least as wide as header
    }

    /// <summary>
    /// Pads the provided value to the column width with trailing spaces.
    /// </summary>
    /// <param name="value">Value to format.</param>
    /// <returns>The padded value.</returns>
    public string GetHeaderValue(string value)
    {
        return value.PadRight(_width);
    }

    /// <summary>
    /// Pads the value to the column width and prefixes it with the configured left offset.
    /// Values exceeding the column width are truncated and suffixed with "..".
    /// </summary>
    /// <param name="value">Value to format.</param>
    /// <returns>The padded and offset-adjusted value.</returns>
    /// <remarks>
    /// Output format: (spaces * offset) + value + (spaces to fill column width)
    /// </remarks>
    public string GetBodyValue(string value)
    {
        if (value.Length > _width)
        {
            value = value[..(_width - 2)] + "..";
        }

        return $"{new string(' ', _leftOffset)}{value.PadRight(_width)}";
    }

}