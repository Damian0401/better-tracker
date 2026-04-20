namespace BetterTracker.Contracts;

public sealed record GetNoteByIdResponse
{
    public required GetNoteByIdDto Note { get; init; }
}

public sealed record GetNoteByIdDto
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required string Content { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required DateTimeOffset UpdatedAt { get; init; }
}
