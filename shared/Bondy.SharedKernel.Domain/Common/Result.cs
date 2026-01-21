namespace Bondy.SharedKernel.Application.Common;

public class Result
{
    protected Result(bool isSuccess, string? successCode, Error error)
    {
        IsSuccess = isSuccess;
        SuccessCode = successCode;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Optional success code (e.g. "post.created").
    /// </summary>
    public string? SuccessCode { get; }

    public Error Error { get; }

    public static Result Success(string? successCode = null)
        => new(true, successCode, Error.None);

    public static Result Failure(Error error)
    {
        if (error is null) throw new ArgumentNullException(nameof(error));
        if (error.IsNone) throw new ArgumentException("Failure result must have a non-empty error.", nameof(error));
        return new(false, null, error);
    }

    public static Result<T> Success<T>(T value, string? successCode = null)
        => Result<T>.Success(value, successCode);

    public static Result<T> Failure<T>(Error error)
        => Result<T>.Failure(error);
}

public sealed class Result<T> : Result
{
    private Result(bool isSuccess, string? successCode, T? value, Error error)
        : base(isSuccess, successCode, error)
    {
        Value = value;
    }

    public T? Value { get; }

    public T ValueOrThrow()
    {
        if (IsFailure)
            throw new InvalidOperationException($"Cannot access Value when result is failure: {Error.Code} - {Error.Message}");
        return Value!;
    }

    public static Result<T> Success(T value, string? successCode = null)
        => new(true, successCode, value, Error.None);

    public new static Result<T> Failure(Error error)
    {
        if (error is null) throw new ArgumentNullException(nameof(error));
        if (error.IsNone) throw new ArgumentException("Failure result must have a non-empty error.", nameof(error));
        return new(false, null, default, error);
    }

    public Result<TOut> Map<TOut>(Func<T, TOut> mapper, string? successCode = null)
    {
        if (mapper is null) throw new ArgumentNullException(nameof(mapper));
        return IsSuccess
            ? Result.Success(mapper(Value!), successCode ?? SuccessCode)
            : Result.Failure<TOut>(Error);
    }

    public Result<TOut> Bind<TOut>(Func<T, Result<TOut>> binder)
    {
        if (binder is null) throw new ArgumentNullException(nameof(binder));
        return IsSuccess ? binder(Value!) : Result.Failure<TOut>(Error);
    }
}