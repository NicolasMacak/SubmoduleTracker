namespace SubmoduleTracker.Domain.AlignmentExecution.Exceptions;
public sealed class ForwardCommitCreationException : Exception
{
    public ForwardCommitCreationException(string path) : base("Creating forward commit failed. Resolve mannually. If your branch already points at HEAD of submodule branch, there is nothing to commit, hence this failure.") { }
}