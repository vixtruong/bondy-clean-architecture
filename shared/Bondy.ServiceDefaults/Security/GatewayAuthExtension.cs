using Bondy.SharedKernel.Api.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Bondy.ServiceDefaults.Security;

public static class GatewayAuthExtension
{
    /// <summary>
    /// Adds JWT Bearer auth using symmetric key.
    /// Config:
    /// Jwt:Issuer, Jwt:Audience, Jwt:Key
    /// </summary>
    public static IServiceCollection AddGatewayAuth(this IServiceCollection services)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Gateway";
                options.DefaultChallengeScheme = "Gateway";
                options.DefaultForbidScheme = "Gateway";
            })
            .AddScheme<AuthenticationSchemeOptions, GatewayAuthenticationHandler>("Gateway", options =>
            {

            });

        services.AddAuthorization(options =>
        {
            foreach (var scope in Scopes.All)
            {
                options.AddPolicy(scope, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", scope);
                });
            }
        });

        return services;
    }
}