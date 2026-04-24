namespace BetterTracker.Contracts;

public sealed record UnarchiveJobApplicationRequest
{
    public required Guid Id { get; init; }
}
