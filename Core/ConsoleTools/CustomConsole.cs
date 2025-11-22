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

    public static bool AskYesOrNoQuestion(string question)
    {
        WriteLineColored(question, TextType.Question);
        Console.WriteLine("Napiste \"yes\" pre pokracovanie");

        string? read = Console.ReadLine();

        if (!string.IsNullOrEmpty(read) && read == "yes")
        {
            return true;
        }

        return false;
    }

    public static int? GetIndexFromChoices(List<string> choices, string prompt, string? emptyStringPrompt = null)
    {
        WriteLineColored(prompt, TextType.Question);
        string incorrectRangeErrorMessage = $"Zadajte cislo medzi 1 a {choices.Count}";

        int index = 1; // increased for more intuitive user experience
        foreach (string choice in choices)
        {
            WriteLineColored($"{index}. {choice}", TextType.MundaneText);
            index++;
        }

        if (!string.IsNullOrEmpty(emptyStringPrompt))
        {
            WriteLineColored(emptyStringPrompt, TextType.Question);
        }

        string? stringNumberOption = Console.ReadLine();

        if (string.IsNullOrEmpty(stringNumberOption))
        {
            if (!string.IsNullOrEmpty(emptyStringPrompt))
            {
                return null;
            }

            WriteErrorLine(incorrectRangeErrorMessage);
            GetIndexFromChoices(choices, prompt, emptyStringPrompt);
        }


        if (!int.TryParse(stringNumberOption, out int parsedNumberOption))
        {
            WriteErrorLine(incorrectRangeErrorMessage);
            GetIndexFromChoices(choices, prompt, emptyStringPrompt);
        }

        parsedNumberOption--; // We increased at int index = 1. now we need to decrease because indexes are from 0

        // cant be lower than lowBoundary
        if (parsedNumberOption < 0)
        {
            WriteErrorLine(incorrectRangeErrorMessage);
            GetIndexFromChoices(choices, prompt, emptyStringPrompt);

        }

        // cant be greater than number of choices
        if (parsedNumberOption > choices.Count - 1)
        {
            WriteErrorLine(incorrectRangeErrorMessage);
            GetIndexFromChoices(choices, prompt, emptyStringPrompt);
        }

        return parsedNumberOption;
    }

}