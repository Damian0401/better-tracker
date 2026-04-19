namespace BetterTracker.Contracts;

public sealed record ListMyTagsResponse
{
    public required IReadOnlyList<Dto> Items { get; init; }

    public sealed record Dto
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
    }
}
