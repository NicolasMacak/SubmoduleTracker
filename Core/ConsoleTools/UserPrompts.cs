using SubmoduleTracker.Core.CommonTypes.Menu;
using SubmoduleTracker.Core.ConsoleTools.Constants;

namespace SubmoduleTracker.Core.ConsoleTools;
/// <summary>
/// Defines user interactions with the application
/// </summary>
public sealed class UserPrompts
{

    public const string ReturnToMainMenuPrompt = "Enter \"\" for returning to the main menu";

    /// <summary>
    /// Prints <paramref name="question"/>. Promps user for typing "yes" to confirm
    /// </summary>
    /// <param name="question"></param>
    /// <returns>True when user types yes, false otherwise</returns>
    public static bool AskYesOrNoQuestion(string question)
    {
        CustomConsole.WriteLineColored(question, TextType.Question);
        Console.WriteLine("Type \"yes\" for continuation");

        string? read = CustomConsole.ReadLine();

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

        string incorrectRangeErrorMessage = $"Choose an number between 1 and {choices.Count}";

        while (true)
        {
            CustomConsole.WriteLineColored(prompt, TextType.Question);

            int index = 1; // increased for more intuitive user experience
            foreach (string choice in choices)
            {
                CustomConsole.WriteLineColored($"{index++}. {choice}", TextType.MundaneText);
            }

            if (!string.IsNullOrEmpty(emptyStringPrompt))
            {
                CustomConsole.WriteLineColored(emptyStringPrompt, TextType.Question);
            }

            string? oneBasedChoiceIndexString = CustomConsole.ReadLine();

            if (string.IsNullOrEmpty(oneBasedChoiceIndexString))
            {
                if (!string.IsNullOrEmpty(emptyStringPrompt))
                {
                    return null;
                }

                CustomConsole.ClearAndWriteErrorLine(incorrectRangeErrorMessage);
                continue;
            }

            if (!int.TryParse(oneBasedChoiceIndexString, out int oneBasedChoiceIndex))
            {
                CustomConsole.ClearAndWriteErrorLine(incorrectRangeErrorMessage);
                continue;
            }
            int zeroBasedChoiceIndex = --oneBasedChoiceIndex; // conversion from one-based to zero-based

            // cant be lower than zero OR cant be greater than number of choices
            if (zeroBasedChoiceIndex  < 0 || zeroBasedChoiceIndex  > choices.Count - 1)
            {
                CustomConsole.ClearAndWriteErrorLine(incorrectRangeErrorMessage);
                continue;
            }

            return zeroBasedChoiceIndex;
        }
    }
}