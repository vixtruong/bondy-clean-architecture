namespace ApiGateway.Clients.Dtos;

public sealed class ApiResponse<T>
{
    public bool Success { get; init; }
    public string? Code { get; init; }
    public T? Data { get; init; }
    public object? Error { get; init; }
    public string? Message { get; init; }
}