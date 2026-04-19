namespace BetterTracker.Contracts;

public sealed record GetJobApplicationDropdownsResponse
{
    public required IReadOnlyList<EnumOption> WorkTypes { get; init; }
    public required IReadOnlyList<EnumOption> SalaryTypes { get; init; }
    public required IReadOnlyList<EnumOption> JobApplicationStatuses { get; init; }

    public sealed record EnumOption
    {
        public required int Value { get; init; }
        public required string Name { get; init; }
    }
}
