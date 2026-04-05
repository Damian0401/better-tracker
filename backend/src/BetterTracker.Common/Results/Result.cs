namespace BetterTracker.Common.Results;

public sealed record Result
{
    public bool IsSuccess { get; init; }
    public string[] ErrorMessages { get; init; } = [];

    public static Result Success() => new() { IsSuccess = true };

    public static Result Failure(params string[] errorMessages) => new()
    {
        IsSuccess = false,
        ErrorMessages = errorMessages
    };

    public static Result Failure(IEnumerable<string> errorMessages) => new()
    {
        IsSuccess = false,
        ErrorMessages = errorMessages.ToArray()
    };
}
