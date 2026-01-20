using System.Text;
using System.Text.Json;

namespace ApiGateway.Middlewares;

public sealed class GatewayErrorEnvelopeMiddleware
{
    public async Task InvokeAsync(
        HttpContext context,
        Func<Task> next)
    {
        await next();

        if (context.Response.HasStarted)
            return;

        if (context.Response.ContentLength.HasValue)
            return;

        var status = context.Response.StatusCode;
        if (status < 400)
            return;

        context.Response.ContentType = "application/json";

        var (code, message) = status switch
        {
            401 => ("auth.unauthorized", "Unauthorized"),
            403 => ("auth.forbidden", "Forbidden"),
            404 => ("gateway.not_found", "Route not found"),
            502 => ("gateway.bad_gateway", "Bad gateway"),
            503 => ("gateway.service_unavailable", "Service unavailable"),
            504 => ("gateway.timeout", "Gateway timeout"),
            _ => ("gateway.error", "Gateway error")
        };

        var payload = new
        {
            success = false,
            code,
            data = (object?)null,
            error = new
            {
                code,
                message,
                type = 5,
                meta = (object?)null,
                isNone = false
            },
            message
        };

        var json = JsonSerializer.Serialize(payload);

        context.Response.ContentLength = Encoding.UTF8.GetByteCount(json);

        await context.Response.WriteAsync(json);
    }
}