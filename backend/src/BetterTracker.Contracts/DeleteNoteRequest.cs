namespace BetterTracker.Contracts;

public sealed record DeleteNoteRequest
{
    public required Guid Id { get; init; }
}
