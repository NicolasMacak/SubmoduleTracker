namespace SubmoduleTracker.SubmoduleAlignment;
public sealed class MemoryModule
{
    private string _branchToReturnTo {  get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>True if stashing or changing branch was done. False otherwise</returns>
    public bool SaveCurrentState() // return value is deciding, whether state needs to be loader later
    {
        return true;
    }
}