namespace Bondy.SharedKernel.Application.Authorization.Scopes;

public static class CommentsScopes
{
    public const string Read = "comments.read";
    public const string Create = "comments.create";
    public const string Update = "comments.update";
    public const string Delete = "comments.delete";
    public const string Moderate = "comments.moderate";
    public const string Report = "comments.report";

    public static readonly IReadOnlyCollection<string> All = new[]
    {
        Read, Create, Update, Delete, Moderate, Report
    };
}