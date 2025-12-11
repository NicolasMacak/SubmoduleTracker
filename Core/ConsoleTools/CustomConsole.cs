using SubmoduleTracker.Core.ConsoleTools.Constants;

namespace SubmoduleTracker.Core.ConsoleTools;
/// <summary>
/// CustomConsole for writing colored text
/// </summary>
public static class CustomConsole
{
    public static void ClearAndWriteLine(string? text)
    {
        Console.Clear();
        Console.WriteLine(text);
    }

    /// <summary>
    /// Writes red line
    /// </summary>
    public static void WriteErrorLine(string error)
    {
        WriteLineColored(error, TextType.Error);
    }

    public static void ClearAndWriteErrorLine(string error)
    {
        Console.Clear();
        WriteLineColored(error, TextType.Error);
    }

    /// <summary>
    /// Writes text in provided color. No new line
    /// </summary>
    public static void WriteColored(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write(text);
        Console.ForegroundColor = ConsoleColor.White;
    }

    public static void WriteLineColored(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ForegroundColor = ConsoleColor.White;
    }

    public static string? ReadLine()
    {
        WriteColored("$ ", TextType.MundaneText);
        return Console.ReadLine();
    }
}