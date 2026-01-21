using Bondy.SharedKernel.Application.Authorization.Scopes;

namespace Bondy.SharedKernel.Application.Authorization.Personas;

public static class ModeratorScopes
{
    public static readonly IReadOnlyCollection<string> All =
        UserScopes.All
            .Concat(new[]
            {
                PostsScopes.Moderate,
                CommentsScopes.Moderate,
                ReelsScopes.Moderate
            })
            .Concat(ModerationScopes.All)
            .Distinct()
            .ToArray();
}