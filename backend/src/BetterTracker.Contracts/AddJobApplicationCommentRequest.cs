namespace BetterTracker.Contracts;

public sealed record AddJobApplicationCommentRequest
{
    public required Guid JobApplicationId { get; init; }
    public required string Content { get; init; }
}
