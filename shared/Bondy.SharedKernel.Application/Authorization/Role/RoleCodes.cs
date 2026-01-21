namespace Bondy.SharedKernel.Application.Authorization.Role;

public static class RoleCodes
{
    public const string User = "user";
    public const string Moderator = "moderator";
    public const string Admin = "admin";
    public const string System = "system";

    public static readonly IReadOnlyCollection<string> All = new[]
    {
        User, Moderator, Admin, System
    };
}