namespace BetterTracker.Contracts;

public sealed record CreateJobApplicationResponse
{
    public required Guid Id { get; init; }
}
