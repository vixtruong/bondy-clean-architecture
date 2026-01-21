namespace Bondy.SharedKernel.Application.Authorization.Scopes;

public static class ModerationScopes
{
    public const string ReportsSubmit = "reports.submit";
    public const string ReportsRead = "reports.read";
    public const string ReportsAction = "reports.action";
    public const string BanUser = "moderation.ban.user";
    public const string SuspendUser = "moderation.suspend.user";
    public const string WarnUser = "moderation.warn.user";
    public const string ContentRemove = "moderation.content.remove";

    public static readonly IReadOnlyCollection<string> All = new[]
    {
        ReportsSubmit, ReportsRead, ReportsAction,
        BanUser, SuspendUser, WarnUser, ContentRemove
    };
}