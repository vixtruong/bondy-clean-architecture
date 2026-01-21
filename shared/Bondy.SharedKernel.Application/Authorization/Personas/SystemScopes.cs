using Bondy.SharedKernel.Application.Authorization.Scopes;

namespace Bondy.SharedKernel.Application.Authorization.Personas;

public static class SystemScopes
{
    public static readonly IReadOnlyCollection<string> All =
        new[]
        {
            InternalScopes.All,
            InternalScopes.ApiGatewayHealth,
            DataAnalyticsScopes.SystemMaintenance
        };
}