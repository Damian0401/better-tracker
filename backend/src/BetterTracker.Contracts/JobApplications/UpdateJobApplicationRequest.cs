namespace BetterTracker.Contracts;

public sealed record UpdateJobApplicationRequest
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
    public IReadOnlyList<UpdateJobApplicationSalaryDto>? Salaries { get; init; }
    public IReadOnlyList<string>? Tags { get; init; }
}

public sealed record UpdateJobApplicationSalaryDto
{
    public required int SalaryType { get; init; }
    public decimal? SalaryPost { get; init; }
    public decimal? SalaryCandidate { get; init; }
    public string? Currency { get; init; }
}
