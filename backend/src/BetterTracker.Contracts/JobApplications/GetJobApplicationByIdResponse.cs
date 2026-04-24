namespace BetterTracker.Contracts;

public sealed record GetJobApplicationByIdResponse
{
    public required GetJobApplicationByIdDto JobApplication { get; init; }
}

public sealed record GetJobApplicationByIdDto
{
    public required Guid Id { get; init; }
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
    public required bool IsArchived { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required DateTimeOffset UpdatedAt { get; init; }
    public required IReadOnlyList<string> Tags { get; init; }
    public required IReadOnlyList<GetJobApplicationByIdSalaryDto> Salaries { get; init; }
    public required IReadOnlyList<GetJobApplicationByIdStatusHistoryDto> StatusHistory { get; init; }
    public required IReadOnlyList<GetJobApplicationByIdCommentDto> Comments { get; init; }
}

public sealed record GetJobApplicationByIdSalaryDto
{
    public required int SalaryType { get; init; }
    public decimal? OfferFrom { get; init; }
    public decimal? OfferTo { get; init; }
    public decimal? ExpectedFrom { get; init; }
    public decimal? ExpectedTo { get; init; }
    public string? Currency { get; init; }
}

public sealed record GetJobApplicationByIdStatusHistoryDto
{
    public int? PreviousStatus { get; init; }
    public required int NewStatus { get; init; }
    public required DateTimeOffset ChangedAt { get; init; }
}

public sealed record GetJobApplicationByIdCommentDto
{
    public required Guid Id { get; init; }
    public required string Content { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required DateTimeOffset UpdatedAt { get; init; }
}
