
using System.Text.Json;
using Bondy.ServiceDefaults.Contracts;
using Bondy.ServiceDefaults.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Bondy.SharedKernel.Common;

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

            _logger.LogError(ex, "Unhandled exception. TraceId={TraceId}", traceId);

            // Map exception -> Error (tuỳ bạn mở rộng)
            var err = ex switch
            {
                ArgumentException aex => Error.Validation("validation.argument", aex.Message),
                UnauthorizedAccessException uex => Error.Unauthorized("auth.unauthorized", uex.Message),
                _ => Error.Failure("server.error", "Unexpected error")
            };

            var status = ErrorMapping.ToStatusCode(err.Type);

            context.Response.StatusCode = status;
            context.Response.ContentType = "application/json";

            var payload = new ApiResponse(
                Success: false,
                Code: null,
                Data: null,
                Error: new ApiError(err.Code, err.Message, err.Type.ToString(), err.Meta),
                TraceId: traceId
            );

            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
    }
}