namespace SubmoduleTracker.Core.ConsoleTools;
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
    public static void WriteErrorLine(string? error)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(error);
        Console.ForegroundColor = ConsoleColor.White;
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

    /// <summary>
    /// Writes text in provided color. No new line
    /// </summary>
    public static void WriteHighlighted(string text)
    {
        Console.ForegroundColor = ConsoleColor.Black;
        Console.BackgroundColor = ConsoleColor.White;
        Console.Write(text);
        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.Black;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="upperBoundary"></param>
    /// <param name="lowerBoundary"></param>
    /// <returns>
    /// </returns>
    public static int? ReadAndValidateNumberInput(int upperBoundary, int? lowerBoundary = null)
    {
        string? maybeNumberChoice = Console.ReadLine();

        if (string.IsNullOrEmpty(maybeNumberChoice))
        { // empty input
            return null;
        }

        if (!int.TryParse(maybeNumberChoice , out int parsedNumberOption))
        { // not a number
            return null;
        }

        // cant be lower than lowBoundary
        if (lowerBoundary.HasValue && parsedNumberOption < lowerBoundary.Value)
        {
            return null;
        }

        // cant be greater than upper boundary
        if (parsedNumberOption > upperBoundary)
        {
            return null;
        }

        return parsedNumberOption;

    }

    public static string ReadStringInput()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;

        Console.Write(">> ");
        string? answer = Console.ReadLine();
        Console.ForegroundColor= ConsoleColor.White;

        return string.IsNullOrEmpty(answer) ? null : answer;
    }


    public static bool AskYesOrNoQuestion(string question)
    {
        Console.WriteLine(question);
        Console.WriteLine("Napiste \"yes\" pre pokracovanie");

        string? read = Console.ReadLine();

        if (!string.IsNullOrEmpty(read) && read == "yes")
        {
            return true;
        }

        return false;
    }
}