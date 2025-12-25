using ApiGateway.Auth;
using ApiGateway.Middlewares;
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

        builder.Host.UseSerilog((ctx, services, cfg) =>
        {
            cfg.ReadFrom.Configuration(ctx.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Service", builder.Environment.ApplicationName);
        });

        builder.Configuration
            .AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"swagger.{builder.Environment.EnvironmentName}.json", optional: false, reloadOnChange: true);

        builder.Services.AddGatewayJwtAuth(builder.Configuration);

        builder.Services.AddOcelot(builder.Configuration).AddPolly();
        builder.Services.AddSwaggerForOcelot(builder.Configuration);

        builder.Services.AddTransient<GatewayErrorEnvelopeMiddleware>();

        var app = builder.Build();

        app.Logger.LogInformation("API Gateway running. Env={Env} Urls={Urls}",
            app.Environment.EnvironmentName,
            builder.Configuration["ASPNETCORE_URLS"] ?? string.Join(", ", app.Urls));

        if (app.Environment.IsDevelopment())
        {
            app.UseSwaggerForOcelotUI(opt => opt.PathToSwaggerGenerator = "/swagger/docs");
        }
        else
        {
            app.UseHttpsRedirection();
        }

        app.UseMiddleware<GatewayErrorEnvelopeMiddleware>();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseOcelot().Wait();

        app.Run();
    }
}
