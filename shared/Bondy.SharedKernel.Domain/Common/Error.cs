namespace Bondy.SharedKernel.Domain.Common;

public enum ErrorType
{
    None = 0,
    BadRequest = 1,
    Validation = 2,
    NotFound = 3,
    Conflict = 4,
    Unauthorized = 5,
    Forbidden = 6,
    Failure = 7
}

public sealed record Error(
    string Code,
    string Message,
    ErrorType Type = ErrorType.Failure,
    IReadOnlyDictionary<string, object?>? Meta = null)
{
    public static readonly Error None = new("none", string.Empty, ErrorType.None);

    public bool IsNone => Type == ErrorType.None || string.Equals(Code, None.Code, StringComparison.OrdinalIgnoreCase);

    public static Error Validation(string code, string message, IReadOnlyDictionary<string, object?>? meta = null)
        => new(code, message, ErrorType.Validation, meta);

    public static Error NotFound(string code, string message, IReadOnlyDictionary<string, object?>? meta = null)
        => new(code, message, ErrorType.NotFound, meta);

    public static Error Conflict(string code, string message, IReadOnlyDictionary<string, object?>? meta = null)
        => new(code, message, ErrorType.Conflict, meta);

    public static Error Unauthorized(string code, string message = "Unauthorized", IReadOnlyDictionary<string, object?>? meta = null)
        => new(code, message, ErrorType.Unauthorized, meta);

    public static Error Forbidden(string code, string message = "Forbidden", IReadOnlyDictionary<string, object?>? meta = null)
        => new(code, message, ErrorType.Forbidden, meta);

    public static Error Failure(string code, string message, IReadOnlyDictionary<string, object?>? meta = null)
        => new(code, message, ErrorType.Failure, meta);

    public static Error BadRequest(string code, string message, IReadOnlyDictionary<string, object?>? meta = null)
        => new(code, message, ErrorType.BadRequest, meta);

}