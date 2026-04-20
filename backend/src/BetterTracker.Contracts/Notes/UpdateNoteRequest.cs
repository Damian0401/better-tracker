namespace BetterTracker.Contracts;

public sealed record UpdateNoteRequest
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required string Content { get; init; }
}
