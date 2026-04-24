namespace BetterTracker.Contracts;

public sealed record ArchiveJobApplicationRequest
{
    public required Guid Id { get; init; }
}
