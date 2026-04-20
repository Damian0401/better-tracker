namespace BetterTracker.Contracts;

public sealed record ListJobApplicationsResponse
{
    public required int Total { get; init; }
    public required IReadOnlyList<ListJobApplicationsItemDto> Items { get; init; }
}

public sealed record ListJobApplicationsItemDto
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required string JobTitle { get; init; }
    public required string CompanyName { get; init; }
    public required int WorkType { get; init; }
    public required int CurrentStatus { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required DateTimeOffset UpdatedAt { get; init; }
    public required IReadOnlyList<string> Tags { get; init; }
}
