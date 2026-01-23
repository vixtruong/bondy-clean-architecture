using System.Security.Claims;
namespace ApiGateway.Middlewares.Auth;

public sealed class IdentityForwardingMiddleware
{
    private readonly RequestDelegate _next;

    public IdentityForwardingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 1. Clean spoofable headers first
        context.Request.Headers.Remove("X-Auth-Type");
        context.Request.Headers.Remove("X-Identity-Id");
        context.Request.Headers.Remove("X-Identity-Owner");
        context.Request.Headers.Remove("X-Effective-ScopesAll");
        context.Request.Headers.Remove("X-Role");

        string? authType = null;
        string? identityId = null;
        string? owner = null;
        string[] scopes = Array.Empty<string>();
        string? role = null;

        var user = context.User;
        if (user?.Identity?.IsAuthenticated == true)
        {
            // Prefer explicit claim "auth_type" if handler set it (ApiKey handler may set it)
            authType = user.FindFirst("auth_type")?.Value
                       ?? user.Identity?.AuthenticationType;

            // identity id (sub or nameidentifier)
            identityId = user.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value
                         ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // owner/email
            owner = user.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email)?.Value
                    ?? user.FindFirst(ClaimTypes.Email)?.Value;

            // scopes
            scopes = user.FindAll("scope")
                         .Select(c => c.Value)
                         .Where(v => !string.IsNullOrWhiteSpace(v))
                         .Distinct()
                         .ToArray();

            // role: either claim "role" or ClaimTypes.Role
            role = user.FindAll("role").Select(c => c.Value).Distinct().SingleOrDefault()
                   ?? user.FindFirst(ClaimTypes.Role)?.Value;
        }

        // Fallback: some handlers (or legacy middleware) may place identity in Context.Items
        if (string.IsNullOrWhiteSpace(identityId) && context.Items.TryGetValue("identity:id", out var iid))
            identityId = iid?.ToString();

        if (string.IsNullOrWhiteSpace(owner) && context.Items.TryGetValue("identity:owner", out var iowner))
            owner = iowner?.ToString();

        if (scopes.Length == 0 && context.Items.TryGetValue("identity:scopes", out var iscopes))
        {
            if (iscopes is IEnumerable<string> seq)
                scopes = seq.ToArray();
            else if (iscopes is string s)
                scopes = s.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }

        if (string.IsNullOrWhiteSpace(authType) && context.Items.TryGetValue("identity:type", out var itype))
            authType = itype?.ToString();

        if (string.IsNullOrWhiteSpace(role) && context.Items.TryGetValue("identity:role", out var irole))
            role = irole?.ToString();

        // If we have at least an identity id -> forward headers
        if (!string.IsNullOrWhiteSpace(identityId))
        {
            // remove Authorization to avoid leaking raw tokens to downstream
            context.Request.Headers.Remove("Authorization");

            if (string.IsNullOrWhiteSpace(authType))
                authType = "unknown";

            context.Request.Headers["X-Auth-Type"] = authType;
            context.Request.Headers["X-Identity-Id"] = identityId;

            if (!string.IsNullOrWhiteSpace(owner))
                context.Request.Headers["X-Identity-Owner"] = owner;

            if (scopes.Length > 0)
                context.Request.Headers["X-Effective-ScopesAll"] = string.Join(',', scopes);

            if (!string.IsNullOrWhiteSpace(role))
                context.Request.Headers["X-Role"] = role;

            // keep in Items for other middlewares in same pipeline if needed
            context.Items["identity:type"] = authType;
            context.Items["identity:id"] = identityId;
            context.Items["identity:owner"] = owner;
            context.Items["identity:scopes"] = scopes;
            if (!string.IsNullOrWhiteSpace(role))
                context.Items["identity:role"] = role;
        }

        await _next(context);
    }
}
