using System.Security.Claims;
using System.Text.Json;
using Bondy.ServiceDefaults.Http;
using Bondy.SharedKernel.Common;
using Bondy.SharedKernel.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace Bondy.ServiceDefaults.Security;

public sealed class GatewayAuthenticationHandler
    : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly JsonSerializerOptions _jsonOptions;

    public GatewayAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IOptions<JsonOptions> jsonOptions)
        : base(options, logger, encoder)
    {
        _jsonOptions = jsonOptions.Value.JsonSerializerOptions;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var headers = Request.Headers;

        if (!headers.TryGetValue("X-Auth-Type", out var authType) ||
            string.IsNullOrWhiteSpace(authType))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (!headers.TryGetValue("X-Identity-Id", out var id) ||
            !headers.TryGetValue("X-Identity-Owner", out var owner))
        {
            Logger.LogWarning("Gateway auth failed: missing identity headers");
            return Task.FromResult(AuthenticateResult.Fail("Invalid gateway identity headers"));
        }

        var scopesRaw = headers.TryGetValue("X-Effective-Scopes", out var s)
            ? s.ToString()
            : string.Empty;

        var scopes = scopesRaw
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, id!),
            new Claim(ClaimTypes.Email, owner!),
            new Claim("auth_type", authType!)
        };

        foreach (var scope in scopes)
            claims.Add(new Claim("scope", scope));

        if (headers.TryGetValue("X-Role", out var role) && !string.IsNullOrWhiteSpace(role))
            claims.Add(new Claim(ClaimTypes.Role, role!));

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);

        return Task.FromResult(
            AuthenticateResult.Success(
                new AuthenticationTicket(principal, Scheme.Name)));
    }

    // ─────────────────────────────────────────────
    // 401 Unauthorized
    // ─────────────────────────────────────────────
    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        return WriteErrorResponse(
            StatusCodes.Status401Unauthorized,
            Error.Unauthorized(
                ErrorCodes.Auth.Unauthorized,
                "Unauthorized"));
    }

    // ─────────────────────────────────────────────
    // 403 Forbidden
    // ─────────────────────────────────────────────
    protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        return WriteErrorResponse(
            StatusCodes.Status403Forbidden,
            Error.Forbidden(
                ErrorCodes.Auth.Forbidden,
                "Forbidden"));
    }

    private Task WriteErrorResponse(int statusCode, Error error)
    {
        var traceId = Context.TraceIdentifier;

        var meta = error.Meta is null
            ? new Dictionary<string, object?>()
            : new Dictionary<string, object?>(error.Meta);

        meta["traceId"] = traceId;

        var enrichedError = error with { Meta = meta };

        var payload = new ApiResponse(
            Success: false,
            Code: enrichedError.Code,
            Data: null,
            Error: enrichedError,
            Message: enrichedError.Message
        );

        Response.StatusCode = statusCode;
        Response.ContentType = "application/json";

        return Response.WriteAsync(
            JsonSerializer.Serialize(payload, _jsonOptions));
    }
}
