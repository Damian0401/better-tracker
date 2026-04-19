namespace BetterTracker.Contracts;

public sealed record GetJobApplicationByIdResponse
{
    public required Dto JobApplication { get; init; }

    public sealed record Dto
    {
        public required Guid Id { get; init; }
        public required string Title { get; init; }
        public required string JobTitle { get; init; }
        public string? Description { get; init; }
        public required string CompanyName { get; init; }
        public string? Requirements { get; init; }
        public string? Benefits { get; init; }
        public string? Link { get; init; }
        public string? Technologies { get; init; }
        public string? Experience { get; init; }
        public required int WorkType { get; init; }
        public required int CurrentStatus { get; init; }
        public required DateTimeOffset CreatedAt { get; init; }
        public required DateTimeOffset UpdatedAt { get; init; }
        public required IReadOnlyList<string> Tags { get; init; }
        public required IReadOnlyList<SalaryDto> Salaries { get; init; }
        public required IReadOnlyList<StatusHistoryDto> StatusHistory { get; init; }
        public required IReadOnlyList<CommentDto> Comments { get; init; }
    }

    public sealed record SalaryDto
    {
        public required int SalaryType { get; init; }
        public decimal? SalaryPost { get; init; }
        public decimal? SalaryCandidate { get; init; }
        public string? Currency { get; init; }
    }

    public sealed record StatusHistoryDto
    {
        public int? PreviousStatus { get; init; }
        public required int NewStatus { get; init; }
        public required DateTimeOffset ChangedAt { get; init; }
    }

    public sealed record CommentDto
    {
        public required Guid Id { get; init; }
        public required string Content { get; init; }
        public required DateTimeOffset CreatedAt { get; init; }
        public required DateTimeOffset UpdatedAt { get; init; }
    }
}
