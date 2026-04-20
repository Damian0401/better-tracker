namespace BetterTracker.Contracts;

public sealed record DeleteJobApplicationRequest
{
    public required Guid Id { get; init; }
}
