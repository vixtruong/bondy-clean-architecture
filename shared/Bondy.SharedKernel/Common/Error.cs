namespace Bondy.SharedKernel.Common
{
    public enum ErrorType
    {
        None = 0,
        Validation = 1,
        NotFound = 2,
        Conflict = 3,
        Unauthorized = 4,
        Forbidden = 5,
        Failure = 6
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
    }
}