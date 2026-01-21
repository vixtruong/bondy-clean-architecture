namespace Bondy.SharedKernel.Application.Authorization.Scopes;

public static class PostsScopes
{
    public const string Read = "posts.read";
    public const string Create = "posts.create";
    public const string Update = "posts.update";
    public const string Delete = "posts.delete";
    public const string Publish = "posts.publish";
    public const string Pin = "posts.pin";
    public const string Moderate = "posts.moderate";
    public const string Report = "posts.report";

    public static readonly IReadOnlyCollection<string> All = new[]
    {
        Read, Create, Update, Delete,
        Publish, Pin, Moderate, Report
    };
}