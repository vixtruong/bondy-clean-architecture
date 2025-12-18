using System.Text.Json;
using Bondy.ServiceDefaults.Http;
using Bondy.SharedKernel.Common;
using Bondy.SharedKernel.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Bondy.ServiceDefaults.Middlewares;

public sealed class GlobalExceptionMiddleware : IMiddleware
{
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(ILogger<GlobalExceptionMiddleware> logger)
    {
        _logger = logger;
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

            if (err.Type is ErrorType.Validation or ErrorType.Unauthorized or ErrorType.Forbidden)
            {
                _logger.LogDebug("Expected exception suppressed. TraceId={TraceId} Code={Code} Type={Type}",
                    traceId, err.Code, err.Type);
            }
            else
            {
                _logger.LogError(ex, "Unhandled exception. TraceId={TraceId} Code={Code} Type={Type}",
                    traceId, err.Code, err.Type);
            }

            context.Response.StatusCode = HttpStatusMapper.ToStatusCode(err.Type);
            context.Response.ContentType = "application/json";

            var payload = new ApiResponse(
                Success: false,
                Code: err.Code,
                Data: null,
                Error: err,
                Message: err.Message
            );

            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
    }

    private static Error MapException(Exception ex, string traceId)
    {
        var baseErr = ex switch
        {
            ArgumentException aex => Error.Validation(ErrorCodes.Validation.Argument, aex.Message),

            OperationCanceledException => Error.Failure(ErrorCodes.Server.Cancelled, "Request was cancelled"),

            UnauthorizedAccessException uex => Error.Unauthorized(ErrorCodes.Auth.Unauthorized, uex.Message),

            TimeoutException tex => Error.Failure(ErrorCodes.Server.Timeout, tex.Message),

            HttpRequestException hex => Error.Failure(ErrorCodes.Server.DependencyFailure,
                string.IsNullOrWhiteSpace(hex.Message) ? "Dependency call failed" : hex.Message),

            _ => Error.Failure(ErrorCodes.Server.Error, "Unexpected error")
        };

        var meta = baseErr.Meta is null
            ? new Dictionary<string, object?>()
            : new Dictionary<string, object?>(baseErr.Meta);

        meta["traceId"] = traceId;

        return baseErr with { Meta = meta };
    }
}
