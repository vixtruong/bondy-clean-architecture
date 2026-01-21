namespace Bondy.SharedKernel.Application.Authorization.Scopes;

public static class ReelsScopes
{
    public const string Create = "reels.create";
    public const string Read = "reels.read";
    public const string Update = "reels.update";
    public const string Delete = "reels.delete";
    public const string Comment = "reels.comment";
    public const string Moderate = "reels.moderate";

    public static readonly IReadOnlyCollection<string> All = new[]
    {
        Create, Read, Update, Delete, Comment, Moderate
    };
}