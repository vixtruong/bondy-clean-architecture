using Microsoft.AspNetCore.Builder;
using Serilog;

namespace Bondy.ServiceDefaults.Extensions;

public static class SerilogExtensions
{
    /// <summary>
    /// Configure Serilog from configuration + enrichers.
    /// Call this BEFORE builder.Build().
    /// </summary>
    public static WebApplicationBuilder AddSerilogLogging(
        this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((ctx, services, loggerConfig) =>
        {
            loggerConfig
                .ReadFrom.Configuration(ctx.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Service", builder.Environment.ApplicationName);
        });

        return builder;
    }
}