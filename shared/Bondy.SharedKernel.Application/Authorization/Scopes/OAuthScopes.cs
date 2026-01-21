namespace Bondy.SharedKernel.Application.Authorization.Scopes;

public static class OAuthScopes
{
    public const string Read = "oauth.connections.read";
    public const string Manage = "oauth.connections.manage";

    public static readonly IReadOnlyCollection<string> All = new[]
    {
        Read, Manage
    };
}