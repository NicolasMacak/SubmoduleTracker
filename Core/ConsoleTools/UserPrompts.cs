using SubmoduleTracker.Core.ConsoleTools.Constants;

namespace SubmoduleTracker.Core.ConsoleTools;
/// <summary>
/// Defines user interactions with the application
/// </summary>
public sealed class UserPrompts
{

    public static string ReturnToMainMenuPrompt = "Enter \"\" for returning to the main menu";

    /// <summary>
    /// Prints <paramref name="question"/>. Promps user for typing "yes" to confirm
    /// </summary>
    /// <param name="question"></param>
    /// <returns>True when user types yes, false otherwise</returns>
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

    /// <summary>
    /// Displays a numbered list of choices and reads the user's selection.
    /// Supports an empty input when <paramref name="emptyStringPrompt"/> is provided.
    /// Repeat itself until valid input is provided.
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
    public static int? GetIndexOfUserChoice(List<string> choices, string prompt, string? emptyStringPrompt = null)
    {
        if (choices == null || choices.Count == 0)
        {
            throw new ArgumentNullException("Choices must contain at least one item. What is wrong with you");
        }

        string incorrectRangeErrorMessage = $"Zadajte cislo medzi 1 a {choices.Count}";

        while (true)
        {
            CustomConsole.WriteLineColored(prompt, TextType.Question);

            int index = 1; // increased for more intuitive user experience
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

                CustomConsole.ClearAndWriteErrorLine(incorrectRangeErrorMessage);
                continue;
            }

            if (!int.TryParse(stringNumberOption, out int choiceIndex))
            {
                CustomConsole.ClearAndWriteErrorLine(incorrectRangeErrorMessage);
                continue;
            }

            // cant be lower than lowBoundary && cant be greater than number of choices
            if (choiceIndex < 0 && choiceIndex > choices.Count - 1)
            {
                CustomConsole.ClearAndWriteErrorLine(incorrectRangeErrorMessage);
                continue;
            }

            return --choiceIndex; // We increased at int index = 1. now we need to decrease because indexes are from 0
        }

        }
    }