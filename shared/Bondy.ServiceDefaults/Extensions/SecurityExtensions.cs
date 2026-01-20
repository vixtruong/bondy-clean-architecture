using Bondy.ServiceDefaults.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Bondy.ServiceDefaults.Extensions;

public static class SecurityExtensions
{
    public static IApplicationBuilder UseGatewayIdentity(
        this IApplicationBuilder app)
    {
        return app.UseMiddleware<GatewayIdentityMiddleware>();
    }

    public static IServiceCollection AddUserContext(
        this IServiceCollection services)
    {
        services.AddTransient<GatewayIdentityMiddleware>();
        services.AddHttpContextAccessor();

        services.AddScoped<IUserContext>(sp =>
        {
            var ctx = sp.GetRequiredService<IHttpContextAccessor>()
                .HttpContext;

            return (IUserContext)ctx!.Items[HeaderNames.UserContextItem]!;
        });

        return services;
    }
}

