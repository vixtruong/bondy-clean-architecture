using Bondy.SharedKernel.Application.Authorization.Scopes;

namespace Bondy.SharedKernel.Application.Authorization.Personas;

public static class UserScopes
{
    public static readonly IReadOnlyCollection<string> All =
        ProfileScopes.All
            .Concat(new[]
            {
                PostsScopes.Read,
                PostsScopes.Create,
                PostsScopes.Update,
                PostsScopes.Delete,
                PostsScopes.Report
            })
            .Concat(new[]
            {
                CommentsScopes.Read,
                CommentsScopes.Create,
                CommentsScopes.Update,
                CommentsScopes.Delete,
                CommentsScopes.Report
            })
            .Concat(ReactionsScopes.All)
            .Concat(FriendshipsScopes.All)
            .Concat(FeedSearchScopes.All)
            .Concat(MessagingScopes.All)
            .Concat(NotificationsScopes.All)
            .Concat(new[]
            {
                UploadScopes.Image,
                UploadScopes.Video,
                UploadScopes.File
            })
            .Concat(new[]
            {
                ReelsScopes.Create,
                ReelsScopes.Read,
                ReelsScopes.Comment
            })
            .Concat(CollectionsScopes.All)
            .Distinct()
            .ToArray();
}