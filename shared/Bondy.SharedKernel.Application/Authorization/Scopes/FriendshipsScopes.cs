namespace Bondy.SharedKernel.Application.Authorization.Scopes;

public static class FriendshipsScopes
{
    public const string Request = "friends.request";
    public const string Accept = "friends.accept";
    public const string Decline = "friends.decline";
    public const string Remove = "friends.remove";
    public const string List = "friends.list";
    public const string Follow = "follow";
    public const string Unfollow = "unfollow";
    public const string FollowersRead = "followers.read";
    public const string FollowingRead = "following.read";
    public const string BlocksCreate = "blocks.create";
    public const string BlocksRemove = "blocks.remove";
    public const string BlocksRead = "blocks.read";

    public static readonly IReadOnlyCollection<string> All = new[]
    {
        Request, Accept, Decline, Remove, List,
        Follow, Unfollow,
        FollowersRead, FollowingRead,
        BlocksCreate, BlocksRemove, BlocksRead
    };
}