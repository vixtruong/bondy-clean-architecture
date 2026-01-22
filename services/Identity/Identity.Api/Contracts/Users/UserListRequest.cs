using Bondy.SharedKernel.Application.Querying;

namespace Identity.Api.Contracts.Users;

public sealed record UserListRequest : PagedRequest
{
    public string? EmailContains { get; init; }
    public bool? Active { get; init; }
}