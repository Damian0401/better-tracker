namespace BetterTracker.Contracts;

public sealed record RegisterRequest
{
    public required string UserName { get; init; }
    public required string Login { get; init; }
    public required string Password { get; init; }
}
