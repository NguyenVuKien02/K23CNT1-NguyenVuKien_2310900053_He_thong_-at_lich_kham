namespace PhongKham.Common.Results;

public class Result<T> : Result
{
    public T? Data { get; private init; }

    private Result(bool success, T? data, string? error)
        : base(success, error)
    {
        Data = data;
    }

    public static Result<T> Ok(T data) => new(true, data, null);

    public new static Result<T> Fail(string error) => new(false, default, error);
}
