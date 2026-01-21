using Bondy.SharedKernel.Application.Authorization.Scopes;

namespace Bondy.SharedKernel.Application.Authorization;

public static class ScopesAll
{
    public static readonly IReadOnlyCollection<string> All =
        ProfileScopes.All
            .Concat(AuthScopes.All)
            .Concat(PostsScopes.All)
            .Concat(CommentsScopes.All)
            .Concat(ReactionsScopes.All)
            .Concat(FriendshipsScopes.All)
            .Concat(FeedSearchScopes.All)
            .Concat(MessagingScopes.All)
            .Concat(NotificationsScopes.All)
            .Concat(UploadScopes.All)
            .Concat(ReelsScopes.All)
            .Concat(CollectionsScopes.All)
            .Concat(ModerationScopes.All)
            .Concat(AdminFeatureScopes.All)
            .Concat(AdminApiKeyScopes.All)
            .Concat(MailScopes.All)
            .Concat(PaymentsScopes.All)
            .Concat(InternalScopes.AllScopes)
            .Concat(SessionsScopes.All)
            .Concat(OAuthScopes.All)
            .Concat(DataAnalyticsScopes.All)
            .Distinct()
            .ToArray();
}