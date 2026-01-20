using ApiGateway.Clients.Identity;
using System.Text.RegularExpressions;

namespace ApiGateway.Middlewares.Auth;

public sealed class ApiKeyGatewayMiddleware
{
    private const string ApiKeyScheme = "ApiKey ";
    private readonly RequestDelegate _next;
    private readonly IIdentityClient _identityClient;

    public ApiKeyGatewayMiddleware(RequestDelegate next, IIdentityClient identityClient)
    {
        _next = next;
        _identityClient = identityClient;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            await _next(context);
            return;
        }

        var auth = authHeader.ToString();

        if (!auth.StartsWith(ApiKeyScheme, StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        var rawKey = auth[ApiKeyScheme.Length..].Trim();
        if (string.IsNullOrWhiteSpace(rawKey))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        var result = await _identityClient.ValidateApiKeyAsync(rawKey);
        if (!result.IsSuccess || result.Value is null)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        var apiKey = result.Value;

        if (!apiKey.IsActive ||
            (apiKey.ExpiresAt.HasValue && apiKey.ExpiresAt.Value <= DateTimeOffset.UtcNow))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        // path-based
        if (!string.IsNullOrWhiteSpace(apiKey.AllowedPaths))
        {
            var allowedPaths = apiKey.AllowedPaths.Trim();

            if (allowedPaths != "*" && allowedPaths != "/*")
            {
                var path = context.Request.Path.Value ?? string.Empty;

                var matched = allowedPaths
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Any(p => Regex.IsMatch(path, p, RegexOptions.IgnoreCase));

                if (!matched)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return;
                }
            }
        }

        // required_scopes có thể do config route set trước (optional)
        var requiredScopes =
            context.Items["required_scopes"] as IReadOnlyCollection<string>;

        IReadOnlyCollection<string> effectiveScopes;

        if (requiredScopes is null || requiredScopes.Count == 0)
        {
            effectiveScopes = apiKey.Scopes;
        }
        else
        {
            effectiveScopes = apiKey.Scopes
                .Intersect(requiredScopes)
                .ToArray();

            if (effectiveScopes.Count == 0)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return;
            }
        }

        // Strip spoofable headers
        context.Request.Headers.Remove("X-Auth-Type");
        context.Request.Headers.Remove("X-Identity-Id");
        context.Request.Headers.Remove("X-Identity-Owner");
        context.Request.Headers.Remove("X-Effective-Scopes");
        context.Request.Headers.Remove("X-Role");

        // Attach identity + scopes for downstream services (will be forwarded by Ocelot)
        context.Request.Headers["X-Auth-Type"] = "apikey";
        context.Request.Headers["X-Identity-Id"] = apiKey.Id.ToString();
        context.Request.Headers["X-Identity-Owner"] = apiKey.Owner;
        context.Request.Headers["X-Effective-Scopes"] = string.Join(',', effectiveScopes);

        // keep in Items for other middlewares in the same pipeline
        context.Items["identity:type"] = "apikey";
        context.Items["identity:id"] = apiKey.Id;
        context.Items["identity:owner"] = apiKey.Owner;
        context.Items["identity:scopes"] = effectiveScopes;

        await _next(context);
    }
}
