namespace BlazorModularMonolith.Api.Shared.Common;

public class Result
{
    public bool IsSuccess { get; protected init; }
    public string? Error { get; protected init; }

    protected Result() { }

    public static Result Success() => new() { IsSuccess = true };
    public static Result Failure(string error) => new() { IsSuccess = false, Error = error };
}

public class Result<T> : Result
{
    public T? Value { get; private init; }

    private Result() { }

    public static Result<T> Success(T value) => new() { IsSuccess = true, Value = value };
    public new static Result<T> Failure(string error) => new() { IsSuccess = false, Error = error };
}
