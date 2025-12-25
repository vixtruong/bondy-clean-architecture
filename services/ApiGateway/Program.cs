using ApiGateway.Auth;
using ApiGateway.Middlewares;
using ApiGateway.Ocelot;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;
using Serilog;

namespace ApiGateway;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Logging
        builder.Host.UseSerilog((ctx, services, cfg) =>
        {
            cfg.ReadFrom.Configuration(ctx.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Service", builder.Environment.ApplicationName);
        });

        // Configs
        builder.Configuration
            .AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", false, true)
            .AddJsonFile($"swagger.{builder.Environment.EnvironmentName}.json", false, true);

        // JWT
        builder.Services.AddGatewayJwtAuth(builder.Configuration);

        // Patch public/private routes
        OcelotRoutePatcher.Patch(builder.Configuration, builder.Environment);

        // Ocelot
        builder.Services
            .AddOcelot(builder.Configuration)
            .AddPolly();

        builder.Services.AddSwaggerForOcelot(builder.Configuration);

        builder.Services.AddTransient<GatewayErrorEnvelopeMiddleware>();

        var app = builder.Build();

        app.UseMiddleware<GatewayErrorEnvelopeMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwaggerForOcelotUI(opt =>
            {
                opt.PathToSwaggerGenerator = "/swagger/docs";
            });
        }
        else
        {
            app.UseHttpsRedirection();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseOcelot().Wait();

        app.Run();
    }
}