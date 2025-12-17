namespace Bondy.SharedKernel.Querying;

public record PagedRequest
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;

    // sort: "createdAt", "-createdAt", "email", "-email"
    public string? Sort { get; init; }
}