namespace Bondy.SharedKernel.Application.Authorization.Scopes;

public static class AuthScopes
{
    public const string Login = "auth.login";
    public const string Refresh = "auth.refresh";
    public const string Logout = "auth.logout";
    public const string Register = "auth.register";
    public const string SessionsRead = "auth.sessions.read";
    public const string SessionsRevoke = "auth.sessions.revoke";
    public const string TwoFaSetup = "auth.2fa.setup";
    public const string TwoFaVerify = "auth.2fa.verify";
    public const string PasswordReset = "auth.password.reset";

    public static readonly IReadOnlyCollection<string> All = new[]
    {
        Login, Refresh, Logout, Register,
        SessionsRead, SessionsRevoke,
        TwoFaSetup, TwoFaVerify,
        PasswordReset
    };
}