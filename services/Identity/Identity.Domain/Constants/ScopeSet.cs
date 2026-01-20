using Bondy.SharedKernel.Constants.Authorization;
using Identity.Domain.ValueObjects;

namespace Identity.Domain.Constants;

public static class ScopeSet
{
    // User
    private static readonly IReadOnlyCollection<string> User =
    [
        Scopes.ProfileRead,
        Scopes.ProfileUpdate,
        Scopes.EmailVerify,

        Scopes.AuthLogin,
        Scopes.AuthRefresh,
        Scopes.AuthLogout,
        Scopes.AuthRegister,

        Scopes.PostsRead,
        Scopes.PostsCreate,
        Scopes.PostsUpdate,
        Scopes.PostsDelete,

        Scopes.PaymentsCreate,
        Scopes.PaymentsRead,
    ];

    public static IReadOnlyCollection<Scope> UserScopes =>
        User.Select(s => new Scope(s)).ToArray();


    // Admin
    private static readonly IReadOnlyCollection<string> Admin =
    [
        // kế thừa toàn bộ user
        ..User,

        Scopes.AdminUsersRead,
        Scopes.AdminUsersManage,
        Scopes.AdminSettingsManage,

        Scopes.PaymentsRefund
    ];

    public static IReadOnlyCollection<Scope> AdminScopes =>
        Admin.Select(s => new Scope(s)).ToArray();


    private static readonly IReadOnlyCollection<string> Partner =
    [
        Scopes.PostsRead,
        Scopes.WebhookReceive,
        Scopes.PartnerIntegration
    ];

    public static IReadOnlyCollection<Scope> PartnerScopes =>
        Partner.Select(s => new Scope(s)).ToArray();

    private static readonly IReadOnlyCollection<string> Internal =
    [
        Scopes.InternalAll
    ];

    public static IReadOnlyCollection<Scope> InternalScopes =>
        Internal.Select(s => new Scope(s)).ToArray();
}