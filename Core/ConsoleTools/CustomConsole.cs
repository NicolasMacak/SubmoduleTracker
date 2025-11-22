using SubmoduleTracker.Core.Result;

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
        CustomConsole.WriteLineColored(question, TextType.Question);
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
        CustomConsole.WriteLineColored(prompt, TextType.Question);

        int index = 0;
        foreach (string choice in choices)
        {
            CustomConsole.WriteLineColored($"{index}. {choice}", TextType.MundaneText);
            index++;
        }

        if (!string.IsNullOrEmpty(emptyStringPrompt))
        {
            CustomConsole.WriteLineColored(emptyStringPrompt, TextType.Question);
        }

        string? stringNumberOption = Console.ReadLine();

        if (string.IsNullOrEmpty(stringNumberOption))
        {
            if (!string.IsNullOrEmpty(emptyStringPrompt))
            {
                return null;
            }

            CustomConsole.WriteErrorLine($"Zadajte cislo medzi 0 a {choices.Count - 1}");
            GetIndexFromChoices(choices, prompt, emptyStringPrompt); // Todo. Refactor. Pouziva sa niekde? Ak je empty string prompt sade, tak netreba
        }

        if (!int.TryParse(stringNumberOption, out int parsedNumberOption))
        {
            CustomConsole.WriteErrorLine($"Zadajte cislo medzi 0 a {choices.Count - 1}");
            GetIndexFromChoices(choices, prompt, emptyStringPrompt);
        }

        // cant be lower than lowBoundary
        if (parsedNumberOption < 0)
        {
            CustomConsole.WriteErrorLine($"Zadajte cislo medzi 0 a {choices.Count - 1}");
            GetIndexFromChoices(choices, prompt, emptyStringPrompt);

        }

        // cant be greater than number of choices
        if (parsedNumberOption > choices.Count - 1)
        {
            CustomConsole.WriteErrorLine($"Zadajte cislo medzi 0 a {choices.Count - 1}");
            GetIndexFromChoices(choices, prompt, emptyStringPrompt);
        }

        return parsedNumberOption;
    }

}