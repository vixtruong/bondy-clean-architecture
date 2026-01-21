namespace Bondy.SharedKernel.Application.Authorization.Scopes;

public static class ProfileScopes
{
    public const string Read = "profile.read";
    public const string Update = "profile.update";
    public const string AvatarUpload = "profile.avatar.upload";
    public const string AvatarDelete = "profile.avatar.delete";
    public const string SettingsRead = "profile.settings.read";
    public const string SettingsUpdate = "profile.settings.update";
    public const string EmailVerify = "email.verify";
    public const string DataExport = "data.export";
    public const string DataDelete = "data.delete";

    public static readonly IReadOnlyCollection<string> All = new[]
    {
        Read, Update, AvatarUpload, AvatarDelete,
        SettingsRead, SettingsUpdate, EmailVerify, DataExport, DataDelete
    };
}