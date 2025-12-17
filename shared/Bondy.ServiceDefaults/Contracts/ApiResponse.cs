namespace Bondy.ServiceDefaults.Contracts;

public sealed record ApiResponse(
    bool Success,
    string? Code,
    object? Data,
    ApiError? Error,
    string TraceId);

public sealed record ApiError(
    string Code,
    string Message,
    string Type,
    IReadOnlyDictionary<string, object?>? Meta);