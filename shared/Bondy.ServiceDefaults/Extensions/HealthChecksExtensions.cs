using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

namespace Bondy.ServiceDefaults.Extensions;

public static class HealthChecksExtensions
{
    /// <summary>
    /// Registers basic health checks.
    /// Add optional checks (DB/Redis) in each service where needed.
    /// </summary>
    public static IServiceCollection AddServiceHealthChecks(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var hc = services.AddHealthChecks();

        // Always add a self check
        hc.AddCheck("self", () => HealthCheckResult.Healthy());

        // OPTIONAL EXAMPLES (uncomment per service):
        // var pg = configuration.GetConnectionString("Postgres");
        // if (!string.IsNullOrWhiteSpace(pg))
        //     hc.AddNpgSql(pg, name: "postgres");

        // var sql = configuration.GetConnectionString("SqlServer");
        // if (!string.IsNullOrWhiteSpace(sql))
        //     hc.AddSqlServer(sql, name: "sqlserver");

        // var redis = configuration.GetConnectionString("Redis");
        // if (!string.IsNullOrWhiteSpace(redis))
        //     hc.AddRedis(redis, name: "redis");

        return services;
    }

    /// <summary>
    /// Maps /health and /health/ready endpoints.
    /// </summary>
    public static WebApplication MapServiceHealthChecks(this WebApplication app)
    {
        // Liveness
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        // Readiness (example: only checks tagged "ready" if you use tags)
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = r => r.Tags.Contains("ready"),
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";

                var payload = new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(e => new
                    {
                        name = e.Key,
                        status = e.Value.Status.ToString(),
                        description = e.Value.Description
                    })
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
            }
        });

        return app;
    }
}
