namespace SubmoduleTracker.Core.ConsoleTools;
public static class ConsoleValidation
{
    /// <summary>
    /// Int validation with validation for upper and lower bounday
    /// </summary>
    /// <param name="stringNumberOption">value to be parsed to int</param>
    /// <param name="upperBoundary">number cant be lower than this</param>
    /// <param name="lowerBoundary">number cant be greater than this</param>
    /// <returns>Returns integer when value is valid, null otherwise</returns>
    [Obsolete($"use {nameof(GetIndexFromChoices)} instead")]
    public static int? ReturnValidatedNumberOption(string? stringNumberOption, int upperBoundary, int? lowerBoundary = null)
    {
        if(string.IsNullOrEmpty(stringNumberOption)){ // empty input
            return null;
        }

        if (!int.TryParse(stringNumberOption, out int parsedNumberOption)){ // not a number
            return null;
        }

        // cant be lower than lowBoundary
        if (lowerBoundary.HasValue && parsedNumberOption < lowerBoundary.Value)
        {
            return null;
        }

        // cant be greater than upper boundary
        if(parsedNumberOption >= upperBoundary)
        {
            return null;
        }

        return parsedNumberOption;
    }

    public static int? GetIndexFromChoices(List<string> choices, string prompt, string? emptyStringPrompt = null)
    {
        CustomConsole.WriteLineColored(prompt, TextType.Question);

        int index = 0;
        foreach(string choice in choices)
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