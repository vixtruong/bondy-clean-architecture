using Bondy.ServiceDefaults.Extensions;
using Bondy.ServiceDefaults.Middlewares;
using Bondy.ServiceDefaults.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
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
        builder.Services.AddGatewayAuth();

        // Middleware DI
        builder.Services.AddTransient<GlobalExceptionMiddleware>();

        return builder;
    }

    public static WebApplication UseBondyServiceDefaults(this WebApplication app, WebApplicationBuilder builder, string serviceName)
    {
        app.Logger.LogInformation("{Service} API running. Env={Env} Urls={Urls}",
            serviceName,
            app.Environment.EnvironmentName,
            builder.Configuration["ASPNETCORE_URLS"] ?? string.Join(", ", app.Urls));

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(
                Path.Combine(builder.Environment.ContentRootPath, "uploads")),
            RequestPath = "/uploads"
        });

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
        app.UseAuthentication();
        app.UseAuthorization();

        // Health
        app.MapServiceHealthChecks();
        return app;
    }
}