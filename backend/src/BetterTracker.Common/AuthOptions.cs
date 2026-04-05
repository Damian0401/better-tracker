namespace BetterTracker.Common;

public sealed class AuthOptions
{
    public const string SectionName = "Auth";

    public required string Secret { get; init; }
    public required int TokenTtlMinutes { get; init; }
}
