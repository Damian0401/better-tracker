namespace BetterTracker.Contracts;

public sealed record CreateJobApplicationRequest
{
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
    public IReadOnlyList<CreateJobApplicationSalaryDto>? Salaries { get; init; }
    public IReadOnlyList<string>? Tags { get; init; }
}

public sealed record CreateJobApplicationSalaryDto
{
    public required int SalaryType { get; init; }
    public decimal? OfferFrom { get; init; }
    public decimal? OfferTo { get; init; }
    public decimal? ExpectedFrom { get; init; }
    public decimal? ExpectedTo { get; init; }
    public string? Currency { get; init; }
}
