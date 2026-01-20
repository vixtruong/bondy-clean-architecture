using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApiGateway.Middlewares;

public sealed class JwtGatewayMiddleware
{
    public async Task InvokeAsync(
        HttpContext context)
    {
        // ApiKey middleware → skip
        if (context.Items.ContainsKey("identity:type"))
        {
            //await next();
            return;
        }

        var user = context.User;

        if (user?.Identity?.IsAuthenticated != true)
        {
            //await next();
            return;
        }

        // ─────────────────────────────────────────────
        // Extract identity
        // ─────────────────────────────────────────────
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

        // ─────────────────────────────────────────────
        // Extract scopes
        // ─────────────────────────────────────────────
        var scopes = user
            .FindAll("scope")
            .Select(c => c.Value)
            .Distinct()
            .ToArray();

        // ─────────────────────────────────────────────
        // Extract single role
        // ─────────────────────────────────────────────
        var role = user
            .FindAll("role")
            .Select(c => c.Value)
            .Distinct()
            .SingleOrDefault(); // ⭐ single role

        // ─────────────────────────────────────────────
        // Strip spoofable headers
        // ─────────────────────────────────────────────
        context.Request.Headers.Remove("Authorization");
        context.Request.Headers.Remove("X-Auth-Type");
        context.Request.Headers.Remove("X-Identity-Id");
        context.Request.Headers.Remove("X-Identity-Owner");
        context.Request.Headers.Remove("X-Effective-Scopes");
        context.Request.Headers.Remove("X-Role");

        // ─────────────────────────────────────────────
        // Attach headers
        // ─────────────────────────────────────────────
        context.Request.Headers["X-Auth-Type"] = "jwt";
        context.Request.Headers["X-Identity-Id"] = identityId;
        context.Request.Headers["X-Identity-Owner"] = owner;
        context.Request.Headers["X-Effective-Scopes"] =
            string.Join(',', scopes);

        if (!string.IsNullOrWhiteSpace(role))
        {
            context.Request.Headers["X-Role"] = role;
        }

        //await next();
    }
}
