namespace Bondy.SharedKernel.Application.Authorization.Scopes;

public static class CollectionsScopes
{
    public const string BookmarksCreate = "bookmarks.create";
    public const string BookmarksRead = "bookmarks.read";
    public const string BookmarksDelete = "bookmarks.delete";
    public const string CollectionsCreate = "collections.create";
    public const string CollectionsRead = "collections.read";
    public const string CollectionsUpdate = "collections.update";
    public const string CollectionsDelete = "collections.delete";

    public static readonly IReadOnlyCollection<string> All = new[]
    {
        BookmarksCreate, BookmarksRead, BookmarksDelete,
        CollectionsCreate, CollectionsRead, CollectionsUpdate, CollectionsDelete
    };
}