namespace Bondy.SharedKernel.Application.Authorization.Scopes;

public static class AdminFeatureScopes
{
    public const string UsersRead = "admin.users.read";
    public const string UsersManage = "admin.users.manage";
    public const string SettingsManage = "admin.settings.manage";
    public const string AnalyticsRead = "admin.analytics.read";
    public const string BillingManage = "admin.billing.manage";

    public static readonly IReadOnlyCollection<string> All = new[]
    {
        UsersRead, UsersManage,
        SettingsManage, AnalyticsRead, BillingManage
    };
}