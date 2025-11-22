namespace SubmoduleTracker.Domain.AlignmentExecution.Exceptions;
public sealed class FastForwardMergeFailureException : Exception
{
    public FastForwardMergeFailureException(string path)
    : base($"Fast forward merge failed at path: {path}. Is local history consisent with remote one?") { }
    
}