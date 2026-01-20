using Bondy.ServiceDefaults.Extensions;
using Bondy.ServiceDefaults.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bondy.ServiceDefaults;

public static class ServiceDefaultsRegistrationExtensions
{
    public static WebApplicationBuilder AddBondyServiceDefaults(this WebApplicationBuilder builder)
    {
        // Logging
        builder.AddSerilogLogging();

        // Controllers + Validation
        builder.Services
            .AddControllers()
            .AddServiceValidation();

        // Swagger
        builder.Services.AddServiceSwagger();

        // HealthChecks
        builder.Services.AddServiceHealthChecks(builder.Configuration);

        // Auth
        builder.Services.AddScopesAuthorization();

        // Middleware DI
        builder.Services.AddTransient<GlobalExceptionMiddleware>();
        builder.Services.AddTransient<GatewayClaimsMiddleware>();

        builder.Services.AddUserContext();

        return builder;
    }

    public static WebApplication UseBondyServiceDefaults(this WebApplication app, WebApplicationBuilder builder, string serviceName)
    {
        app.Logger.LogInformation("{Service} API running. Env={Env} Urls={Urls}",
            serviceName,
            app.Environment.EnvironmentName,
            builder.Configuration["ASPNETCORE_URLS"] ?? string.Join(", ", app.Urls));

        // Global exception
        app.UseMiddleware<GlobalExceptionMiddleware>();

        // Swagger
        if (app.Environment.IsDevelopment())
        {
            app.UseServiceSwagger();
        }
        else
        {
            app.UseHttpsRedirection();
        }

        // AuthN/AuthZ
        //app.UseAuthentication();
        app.UseMiddleware<GatewayClaimsMiddleware>();
        app.UseGatewayIdentity();
        app.UseAuthorization();

        // Health
        app.MapServiceHealthChecks();
        return app;
    }
}