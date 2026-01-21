namespace Bondy.SharedKernel.Application.Authorization.Scopes;

public static class SessionsScopes
{
    public const string DevicesManage = "devices.manage";
    public const string SessionsRead = "sessions.read";
    public const string SessionsRevoke = "sessions.revoke";

    public static readonly IReadOnlyCollection<string> All = new[]
    {
        DevicesManage, SessionsRead, SessionsRevoke
    };
}