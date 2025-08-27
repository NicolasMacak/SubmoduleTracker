namespace SubmoduleTracker.ConsoleTools;
public static class ConsoleValidation
{
    public static int? ReturnValidatedNumberOption(string? stringNumberOption, int choicesCount)
    {
        if(string.IsNullOrEmpty(stringNumberOption)){ // empty input
            return null;
        }

        if (!int.TryParse(stringNumberOption, out int parsedNumberOption)){ // not a number
            return null;
        }

        return parsedNumberOption;
    }
}