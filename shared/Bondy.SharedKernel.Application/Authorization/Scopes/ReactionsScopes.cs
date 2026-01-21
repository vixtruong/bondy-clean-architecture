namespace Bondy.SharedKernel.Application.Authorization.Scopes;

public static class ReactionsScopes
{
    public const string Read = "reactions.read";
    public const string Create = "reactions.create";
    public const string Delete = "reactions.delete";

    public static readonly IReadOnlyCollection<string> All = new[]
    {
        Read, Create, Delete
    };
}