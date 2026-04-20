namespace BetterTracker.Contracts;

public sealed record GetJobApplicationDropdownsResponse
{
    public required IReadOnlyList<GetJobApplicationDropdownOption> WorkTypes { get; init; }
    public required IReadOnlyList<GetJobApplicationDropdownOption> SalaryTypes { get; init; }
    public required IReadOnlyList<GetJobApplicationDropdownOption> JobApplicationStatuses { get; init; }
}

public sealed record GetJobApplicationDropdownOption
{
    public required int Value { get; init; }
    public required string Name { get; init; }
}
