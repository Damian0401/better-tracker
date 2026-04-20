namespace BetterTracker.Contracts;

public sealed record AuthResponse
{
    public required string Token { get; init; }
    public required Guid UserId { get; init; }
    public required string UserName { get; init; }
}
