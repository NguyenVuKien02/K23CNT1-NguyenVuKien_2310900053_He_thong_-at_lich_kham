namespace PhongKham.Common.Results;

public class Result
{
    public bool Success { get; protected init; }

    public string? Error { get; protected init; }

    protected Result(bool success, string? error)
    {
        Success = success;
        Error = error;
    }

    public static Result Ok() => new(true, null);

    public static Result Fail(string error) => new(false, error);

    public static Result<T> Ok<T>(T data) => Result<T>.Ok(data);

    public static Result<T> Fail<T>(string error) => Result<T>.Fail(error);
}
