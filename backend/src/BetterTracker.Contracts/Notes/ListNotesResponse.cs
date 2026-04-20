namespace BetterTracker.Contracts;

public sealed record ListNotesResponse
{
    public required int Total { get; init; }
    public required IReadOnlyList<ListNotesItemDto> Items { get; init; }
}

public sealed record ListNotesItemDto
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required string Content { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required DateTimeOffset UpdatedAt { get; init; }
}
