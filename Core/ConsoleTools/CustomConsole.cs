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
    public static void WriteErrorLine(string error)
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

    /// <summary>
    /// Prints <paramref name="question"/>. Promps user for typing "yes" to confirm
    /// </summary>
    /// <param name="question"></param>
    /// <returns>True when user types yes, false otherwise</returns>
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

    /// <summary>
    /// Displays a numbered list of choices and reads the user's selection.
    /// Supports an empty input when <paramref name="emptyStringPrompt"/> is provided.
    /// </summary>
    /// <param name="choices">
    /// The list of selectable options. Displayed to the user as items 1..n.
    /// </param>
    /// <param name="prompt">
    /// The main prompt shown before listing the choices.
    /// </param>
    /// <param name="emptyStringPrompt">
    /// Optional message shown when empty input is allowed.  
    /// When set, an empty input returns <c>null</c>.
    /// </param>
    /// <returns>
    /// <see cref="ResultCode.Success"/> When picks between proposed options, <br></br>
    /// <see cref="ResultCode.EmptyInput"/> When <paramref name="emptyStringPrompt"/> is not null and user enters empty string
    /// </returns>
    /// <remarks>
    /// The user sees options numbered from 1 to <c>choices.Count</c>. Internally,
    /// the method returns a zero-based index.
    /// </remarks>
    public static ModelResult<int> GetIndexOfUserChoice(List<string> choices, string prompt, string? emptyStringPrompt = null)
    {
        if (choices == null || choices.Count == 0)
        {
            throw new ArgumentNullException("Choices must contain at least one item. What is wrong with you");
        }

        while (true)
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
                    return new ModelResult<int>().With(ResultCode.EmptyInput);
                }

                WriteErrorLine(incorrectRangeErrorMessage);
                continue;
            }

            if (!int.TryParse(stringNumberOption, out int choiceIndex))
            {
                WriteErrorLine(incorrectRangeErrorMessage);
                continue;
            }

            choiceIndex--; // We increased at int index = 1. now we need to decrease because indexes are from 0

            // cant be lower than lowBoundary
            if (choiceIndex < 0)
            {
                WriteErrorLine(incorrectRangeErrorMessage);
                continue;
            }

            // cant be greater than number of choices
            if (choiceIndex > choices.Count - 1)
            {
                WriteErrorLine(incorrectRangeErrorMessage);
                continue;
            }

            return new ModelResult<int>().WithSuccess(choiceIndex);
        }
    }
}