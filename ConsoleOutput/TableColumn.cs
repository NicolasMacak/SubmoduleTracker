using SubmoduleTracker.Core;

namespace SubmoduleTracker.ConsoleOutput;
public sealed class TableColumn
{
    public string Name { get; }

    public int Width { get; }

    public TableColumn(string name, int width)
    {
        Name = name;
        Width = width;
    }

    public static int CalculateColumnWidth(string columnName, int longestValueLength)
    {
        int min = Math.Min(longestValueLength, TableConstants.MaxColumnWidth); // as less as required

        return Math.Max(min, columnName.Length); // but more than length of the column name
    }

    public string GetPrintableHeader()
    {
        return Name.PadRight(Width);
    }

    public string GetPrintableValue(string value, int offset)
    {
        return $"{new string(' ', offset)}{value.PadRight(Width)}"
        ;
    }
}