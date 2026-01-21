namespace Bondy.SharedKernel.Application.Authorization.Scopes;

public static class NotificationsScopes
{
    public const string Read = "notifications.read";
    public const string MarkRead = "notifications.mark.read";
    public const string SettingsRead = "notifications.settings.read";
    public const string SettingsUpdate = "notifications.settings.update";

    public static readonly IReadOnlyCollection<string> All = new[]
    {
        Read, MarkRead, SettingsRead, SettingsUpdate
    };
}