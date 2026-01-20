using Microsoft.AspNetCore.Http;

namespace Bondy.ServiceDefaults.Security;

public sealed class GatewayIdentityMiddleware : IMiddleware
{
    public async Task InvokeAsync(
        HttpContext context,
        RequestDelegate next)
    {
        if (!context.Request.Headers.TryGetValue(HeaderNames.AuthType, out var authType))
        {
            await next(context);
            return;
        }

        var scopes = context.Request.Headers[HeaderNames.EffectiveScopes]
            .ToString()
            .Split(',', StringSplitOptions.RemoveEmptyEntries);

        string? role = null;

        if (authType == AuthTypes.Jwt &&
            context.Request.Headers.TryGetValue(HeaderNames.Role, out var r))
        {
            role = r.ToString();
        }

        context.Items[HeaderNames.UserContextItem] = new UserContext
        {
            AuthType = authType!,
            IdentityId = context.Request.Headers[HeaderNames.IdentityId]!,
            Owner = context.Request.Headers[HeaderNames.IdentityOwner]!,
            Scopes = scopes,
            Role = role
        };

        await next(context);
    }
}