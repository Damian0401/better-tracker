namespace BetterTracker.Common.Results;

public sealed record Result<T>
{
    public bool IsSuccess { get; init; }
    public string[] ErrorMessages { get; init; } = [];
    public T? Data { get; init; }

    public static Result<T> Success(T data) => new()
    {
        IsSuccess = true,
        Data = data
    };

    public static Result<T> Failure(params string[] errorMessages) => new()
    {
        IsSuccess = false,
        ErrorMessages = errorMessages
    };

    public static Result<T> Failure(IEnumerable<string> errorMessages) => new()
    {
        IsSuccess = false,
        ErrorMessages = errorMessages.ToArray()
    };
}
