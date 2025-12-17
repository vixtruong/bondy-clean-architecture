using Bondy.SharedKernel.Querying;

namespace Identity.Contracts.Users;

public sealed record UserListRequest : PagedRequest
{
    public string? EmailContains { get; init; }
    public bool? Active { get; init; }
}