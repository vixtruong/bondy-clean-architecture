
namespace Bondy.SharedKernel.Authorization;

public static class Scopes
{
    // Profile
    public const string ProfileRead = "profile.read";
    public const string ProfileUpdate = "profile.update";
    public const string EmailVerify = "email.verify";

    // Auth
    public const string AuthLogin = "auth.login";
    public const string AuthRefresh = "auth.refresh";
    public const string AuthLogout = "auth.logout";
    public const string AuthRegister = "auth.logout";

    // Posts
    public const string PostsRead = "posts.read";
    public const string PostsCreate = "posts.create";
    public const string PostsUpdate = "posts.update";
    public const string PostsDelete = "posts.delete";

    // Payments
    public const string PaymentsCreate = "payments.create";
    public const string PaymentsRead = "payments.read";
    public const string PaymentsRefund = "payments.refund";

    // Admin
    public const string AdminUsersRead = "admin.users.read";
    public const string AdminUsersManage = "admin.users.manage";
    public const string AdminSettingsManage = "admin.settings.manage";

    // Internal / partner
    public const string InternalAll = "internal.*";
    public const string WebhookReceive = "webhook.receive";
    public const string PartnerIntegration = "partner.integration";

    public static readonly IReadOnlyCollection<string> All =
    [
        ProfileRead,
        ProfileUpdate,
        EmailVerify,

        AuthLogin,
        AuthRefresh,
        AuthLogout,
        AuthRegister,

        PostsRead,
        PostsCreate,
        PostsUpdate,
        PostsDelete,

        PaymentsCreate,
        PaymentsRead,
        PaymentsRefund,

        AdminUsersRead,
        AdminUsersManage,
        AdminSettingsManage,

        InternalAll,
        WebhookReceive,
        PartnerIntegration
    ];
}