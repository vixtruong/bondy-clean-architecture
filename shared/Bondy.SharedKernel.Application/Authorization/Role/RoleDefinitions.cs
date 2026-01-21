using Bondy.SharedKernel.Application.Authorization.Personas;

namespace Bondy.SharedKernel.Application.Authorization.Role;

public sealed record RoleDefinition(
    string Code,
    string Name,
    IReadOnlyCollection<string> Scopes
);

public static class RoleDefinitions
{
    public static readonly RoleDefinition User =
        new(
            RoleCodes.User,
            "User",
            UserScopes.All
        );

    public static readonly RoleDefinition Moderator =
        new(
            RoleCodes.Moderator,
            "Moderator",
            ModeratorScopes.All
        );

    public static readonly RoleDefinition Admin =
        new(
            RoleCodes.Admin,
            "Administrator",
            AdminScopes.All
        );

    public static readonly RoleDefinition System =
        new(
            RoleCodes.System,
            "System",
            SystemScopes.All
        );

    public static readonly IReadOnlyCollection<RoleDefinition> All =
        new[] { User, Moderator, Admin, System };
}