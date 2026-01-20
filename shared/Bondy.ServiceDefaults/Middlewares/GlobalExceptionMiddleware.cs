using System.Text.Json;
using Bondy.ServiceDefaults.Http;
using Bondy.SharedKernel.Common;
using Bondy.SharedKernel.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bondy.ServiceDefaults.Middlewares;

public sealed class GlobalExceptionMiddleware : IMiddleware
{
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public GlobalExceptionMiddleware(
        ILogger<GlobalExceptionMiddleware> logger,
        IOptions<JsonOptions> jsonOptions)
    {
        _logger = logger;
        _jsonOptions = jsonOptions.Value.JsonSerializerOptions;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var traceId = context.TraceIdentifier;
            var err = MapException(ex, traceId);

            if (err.Type != ErrorType.Validation)
                _logger.LogError(ex, "Unhandled exception. TraceId={TraceId} Code={Code} Type={Type}", traceId, err.Code, err.Type);
            else
                _logger.LogDebug("Validation exception suppressed. TraceId={TraceId} Code={Code}", traceId, err.Code);

            context.Response.StatusCode = err.Type.ToStatusCode();
            context.Response.ContentType = "application/json";

            var payload = new ApiResponse(
                Success: false,
                Code: err.Code,
                Data: null,
                Error: err,
                Message: err.Message
            );

            await context.Response.WriteAsync(JsonSerializer.Serialize(payload, _jsonOptions));
        }
    }

    private static Error MapException(Exception ex, string traceId) 
    {
        var baseErr = ex switch
        {
            ArgumentException aex => Error.Validation(ErrorCodes.Validation.Argument, aex.Message),
            UnauthorizedAccessException uex => Error.Unauthorized(ErrorCodes.Auth.Unauthorized, uex.Message),
            _ => Error.Failure(ErrorCodes.Server.Error, "Unexpected error")
        };

        var meta = baseErr.Meta is null
            ? new Dictionary<string, object?>()
            : new Dictionary<string, object?>(baseErr.Meta);

        meta["traceId"] = traceId;

        return baseErr with { Meta = meta };
    }
}
