namespace Bondy.SharedKernel.Application.Authorization.Scopes;

public static class FeedSearchScopes
{
    public const string FeedRead = "feed.read";
    public const string FeedPersonalize = "feed.personalize";
    public const string SearchUsers = "search.users";
    public const string SearchPosts = "search.posts";
    public const string SearchTags = "search.tags";
    public const string HashtagsRead = "hashtags.read";
    public const string TrendsRead = "trends.read";

    public static readonly IReadOnlyCollection<string> All = new[]
    {
        FeedRead, FeedPersonalize,
        SearchUsers, SearchPosts, SearchTags,
        HashtagsRead, TrendsRead
    };
}