namespace SubmoduleTracker.Extensions;
public static class HashSetExtensions
{
    public static void AddRange(this HashSet<string> hashset, IEnumerable<string> items)
    {
        foreach(string item in items)
        {
            hashset.Add(item);
        }
    }
}