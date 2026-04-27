namespace BetterTracker.Contracts;

public sealed record GetJobApplicationStatisticsResponse
{
    public required int Total { get; init; }
    public required IReadOnlyList<GetJobApplicationStatisticsStatusCountDto> StatusCounts { get; init; }
}

public sealed record GetJobApplicationStatisticsStatusCountDto
{
    public required int Status { get; init; }
    public required int Count { get; init; }
}
