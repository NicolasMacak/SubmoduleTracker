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
        if(parsedNumberOption > upperBoundary)
        {
            return null;
        }

        return parsedNumberOption;
    }
}