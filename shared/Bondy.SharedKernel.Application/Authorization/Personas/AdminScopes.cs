using Bondy.SharedKernel.Application.Authorization.Scopes;

namespace Bondy.SharedKernel.Application.Authorization.Personas;

public static class AdminScopes
{
    public static readonly IReadOnlyCollection<string> All =
        ModeratorScopes.All
            .Concat(AdminFeatureScopes.All)
            .Concat(AdminApiKeyScopes.All)
            .Concat(MailScopes.All)
            .Concat(PaymentsScopes.All)
            .Concat(DataAnalyticsScopes.All)
            .Concat(InternalScopes.AllScopes)
            .Distinct()
            .ToArray();
}