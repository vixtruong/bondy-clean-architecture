namespace Bondy.ServiceDefaults.Security;

public sealed class UserContext : IUserContext
{
    public string AuthType { get; init; } = default!;
    public string IdentityId { get; init; } = default!;
    public string Owner { get; init; } = default!;
    public IReadOnlyCollection<string> Scopes { get; init; } =
        Array.Empty<string>();
    public string? Role { get; init; }
}