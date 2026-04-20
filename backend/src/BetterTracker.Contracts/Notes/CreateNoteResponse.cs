namespace BetterTracker.Contracts;

public sealed record CreateNoteResponse
{
    public required Guid Id { get; init; }
}