namespace Bondy.SharedKernel.Application.Authorization.Scopes;

public static class AdminApiKeyScopes
{
    public const string Create = "admin.apikeys.create";
    public const string Rotate = "admin.apikeys.rotate";
    public const string Revoke = "admin.apikeys.revoke";
    public const string Update = "admin.apikeys.update";

    public static readonly IReadOnlyCollection<string> All = new[]
    {
        Create, Rotate, Revoke, Update
    };
}