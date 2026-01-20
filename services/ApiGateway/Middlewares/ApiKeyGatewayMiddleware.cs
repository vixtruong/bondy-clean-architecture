using ApiGateway.Clients.Identity;
using System.Text.RegularExpressions;

namespace ApiGateway.Middlewares;

public sealed class ApiKeyGatewayMiddleware
{
    private const string ApiKeyScheme = "ApiKey ";

    private readonly IIdentityClient _identityClient;

    public ApiKeyGatewayMiddleware(IIdentityClient identityClient)
    {
        _identityClient = identityClient;
    }

    public async Task InvokeAsync(
        HttpContext context)
    {
        // ─────────────────────────────────────────────
        // Only handle ApiKey auth scheme
        // ─────────────────────────────────────────────
        if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            //await next();
            return;
        }

        var auth = authHeader.ToString();

        if (!auth.StartsWith(ApiKeyScheme, StringComparison.OrdinalIgnoreCase))
        {
            //await next();
            return;
        }

        // ─────────────────────────────────────────────
        // Extract raw API key
        // ─────────────────────────────────────────────
        var rawKey = auth[ApiKeyScheme.Length..].Trim();
        if (string.IsNullOrWhiteSpace(rawKey))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        // ─────────────────────────────────────────────
        // Validate API key via Identity service
        // ─────────────────────────────────────────────
        var result = await _identityClient.ValidateApiKeyAsync(rawKey);
        if (!result.IsSuccess || result.Value is null)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        var apiKey = result.Value;

        if (!apiKey.IsActive ||
            (apiKey.ExpiresAt.HasValue &&
             apiKey.ExpiresAt.Value <= DateTimeOffset.UtcNow))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        // ─────────────────────────────────────────────
        // Path-based authorization
        // ─────────────────────────────────────────────
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

        // ─────────────────────────────────────────────
        // Scope authorization (Gateway-level)
        // RequiredScopes should be injected earlier
        // (e.g. from Ocelot route config)
        // ─────────────────────────────────────────────
        var requiredScopes =
            context.Items["required_scopes"] as IReadOnlyCollection<string>;

        IReadOnlyCollection<string> effectiveScopes;

        if (requiredScopes is null || requiredScopes.Count == 0)
        {
            // No restriction → all scopes are effective
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

        // ─────────────────────────────────────────────
        // 6. Strip internal headers (防 spoof)
        // ─────────────────────────────────────────────
        context.Request.Headers.Remove("X-Auth-Type");
        context.Request.Headers.Remove("X-Identity-Id");
        context.Request.Headers.Remove("X-Identity-Owner");
        context.Request.Headers.Remove("X-Effective-Scopes");

        // ─────────────────────────────────────────────
        // 7. Attach identity + scopes for downstream services
        // ─────────────────────────────────────────────
        context.Request.Headers["X-Auth-Type"] = "apikey";
        context.Request.Headers["X-Identity-Id"] = apiKey.Id.ToString();
        context.Request.Headers["X-Identity-Owner"] = apiKey.Owner;
        context.Request.Headers["X-Effective-Scopes"] =
            string.Join(',', effectiveScopes);

        // Optional: keep in Items for other middlewares
        context.Items["identity:type"] = "apikey";
        context.Items["identity:id"] = apiKey.Id;
        context.Items["identity:owner"] = apiKey.Owner;
        context.Items["identity:scopes"] = effectiveScopes;

        //await next();
    }
}
