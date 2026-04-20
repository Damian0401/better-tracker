namespace BetterTracker.Contracts;

public sealed record ListMyTagsResponse
{
    public required IReadOnlyList<ListMyTagsItemDto> Items { get; init; }
}

public sealed record ListMyTagsItemDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}
