using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Bondy.ServiceDefaults.Middlewares;

public class GatewayClaimsMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!context.Request.Headers.TryGetValue("X-Effective-Scopes", out var rawScopes))
        {
            await next(context);
            return;
        }

        var scopes = rawScopes
            .ToString()
            .Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (scopes.Length == 0)
        {
            await next(context);
            return;
        }

        var gatewayIdentity = new ClaimsIdentity(
            authenticationType: "gateway",
            nameType: ClaimTypes.Name,
            roleType: ClaimTypes.Role);

        foreach (var scope in scopes)
        {
            gatewayIdentity.AddClaim(new Claim("scope", scope));
        }

        context.User = new ClaimsPrincipal(gatewayIdentity);

        await next(context);
    }
}
