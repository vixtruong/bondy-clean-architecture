using Bondy.SharedKernel.Domain.Common;

namespace Bondy.ServiceDefaults.Http;

public sealed record ApiResponse<T>(
    bool Success,
    string Code,
    T? Data,
    object? Error,
    string Message)
{
    public static ApiResponse<T> From(Result<T> result)
    {
        if (result is null) throw new ArgumentNullException(nameof(result));

        if (result.IsSuccess)
        {
            return new(
                Success: true,
                Code: result.SuccessCode ?? "OK",
                Data: result.Value,
                Error: null,
                Message: string.Empty
            );
        }

        return new(
            Success: false,
            Code: result.Error.Code,
            Data: default,
            Error: result.Error,
            Message: result.Error.Message
        );
    }
}

public sealed record ApiResponse(
    bool Success,
    string Code,
    object? Data,
    object? Error,
    string Message)
{
    public static ApiResponse From(Result result)
    {
        if (result is null) throw new ArgumentNullException(nameof(result));

        if (result.IsSuccess)
        {
            return new(
                Success: true,
                Code: result.SuccessCode ?? "OK",
                Data: null,
                Error: null,
                Message: string.Empty
            );
        }

        return new(
            Success: false,
            Code: result.Error.Code,
            Data: null,
            Error: result.Error,
            Message: result.Error.Message
        );
    }
}