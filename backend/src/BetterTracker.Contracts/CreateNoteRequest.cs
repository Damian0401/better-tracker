namespace BetterTracker.Contracts;

public sealed record CreateNoteRequest
{
    public required string Title { get; init; }
    public required string Content { get; init; }
}
