namespace BetterTracker.Contracts;

public sealed record ErrorResponse
{
    public required string[] Errors { get; init; }
}
