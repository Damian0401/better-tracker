namespace BetterTracker.Contracts;

public sealed record DeleteJobApplicationCommentRequest
{
    public required Guid Id { get; init; }
}
