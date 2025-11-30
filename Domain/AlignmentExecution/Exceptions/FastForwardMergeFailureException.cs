namespace SubmoduleTracker.Domain.AlignmentExecution.Exceptions;
public sealed class FastForwardMergeFailureException : Exception
{
    public FastForwardMergeFailureException(string path)
    : base($"Fast forward merge failed at path: {path}. Resolve manually.") { }
    
}