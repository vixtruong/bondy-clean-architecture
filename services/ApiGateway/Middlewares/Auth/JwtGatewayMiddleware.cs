using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApiGateway.Middlewares.Auth;

public sealed class JwtGatewayMiddleware
{
    private readonly RequestDelegate _next;

    public JwtGatewayMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Nếu ApiKey đã xử lý → skip
        if (context.Items.ContainsKey("identity:type"))
        {
            await _next(context);
            return;
        }

        var user = context.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            await _next(context);
            return;
        }

        var identityId =
            user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(identityId))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        var owner =
            user.FindFirst(JwtRegisteredClaimNames.Email)?.Value
            ?? user.FindFirst(ClaimTypes.Email)?.Value
            ?? identityId;

        var scopes = user
            .FindAll("scope")
            .Select(c => c.Value)
            .Distinct()
            .ToArray();

        var role = user
            .FindAll("role")
            .Select(c => c.Value)
            .Distinct()
            .SingleOrDefault();

        // Strip spoofable headers
        context.Request.Headers.Remove("Authorization");
        context.Request.Headers.Remove("X-Auth-Type");
        context.Request.Headers.Remove("X-Identity-Id");
        context.Request.Headers.Remove("X-Identity-Owner");
        context.Request.Headers.Remove("X-Effective-Scopes");
        context.Request.Headers.Remove("X-Role");

        // Attach headers
        context.Request.Headers["X-Auth-Type"] = "jwt";
        context.Request.Headers["X-Identity-Id"] = identityId;
        context.Request.Headers["X-Identity-Owner"] = owner;
        context.Request.Headers["X-Effective-Scopes"] = string.Join(',', scopes);

        if (!string.IsNullOrWhiteSpace(role))
        {
            context.Request.Headers["X-Role"] = role;
            context.Items["identity:role"] = role;
        }

        // keep in Items for other middlewares
        context.Items["identity:type"] = "jwt";
        context.Items["identity:id"] = identityId;
        context.Items["identity:owner"] = owner;
        context.Items["identity:scopes"] = scopes;

        await _next(context);
    }
}
