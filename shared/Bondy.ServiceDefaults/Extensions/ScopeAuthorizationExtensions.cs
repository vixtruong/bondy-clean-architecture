using Bondy.SharedKernel.Constants.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Bondy.ServiceDefaults.Extensions;

public static class ScopeAuthorizationExtensions
{
    /// <summary>
    /// Adds JWT Bearer auth using symmetric key.
    /// Config:
    /// Jwt:Issuer, Jwt:Audience, Jwt:Key
    /// </summary>
    public static IServiceCollection AddScopesAuthorization(
        this IServiceCollection services)
    {
        //var issuer = configuration["Jwt:Issuer"];
        //var audience = configuration["Jwt:Audience"];
        //var key = configuration["Jwt:Secret"];

        //if (string.IsNullOrWhiteSpace(key))
        //    throw new InvalidOperationException("Missing Jwt:Key in configuration.");

        //var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

        //services
        //    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //    .AddJwtBearer(options =>
        //    {
        //        options.RequireHttpsMetadata = false; // set true in prod with HTTPS
        //        options.TokenValidationParameters = new TokenValidationParameters
        //        {
        //            ValidateIssuer = !string.IsNullOrWhiteSpace(issuer),
        //            ValidIssuer = issuer,

        //            ValidateAudience = !string.IsNullOrWhiteSpace(audience),
        //            ValidAudience = audience,

        //            ValidateIssuerSigningKey = true,
        //            IssuerSigningKey = signingKey,

        //            ValidateLifetime = true,
        //            ClockSkew = TimeSpan.FromSeconds(30)
        //        };
        //    });

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