using ApiGateway.Auth;
using ApiGateway.Clients.Identity;
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

        builder.Services.AddTransient<ApiKeyGatewayMiddleware>();
        builder.Services.AddTransient<JwtGatewayMiddleware>();
        builder.Services.AddTransient<GatewayErrorEnvelopeMiddleware>();

        builder.Services.AddHttpClient<IIdentityClient, IdentityClient>(client =>
        {
            client.BaseAddress = new Uri(
                builder.Configuration["Services:Identity"]!);

            client.Timeout = TimeSpan.FromSeconds(5);
        });


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

        app.UseAuthentication();
        app.UseAuthorization();

        var ocelotPipeline = new OcelotPipelineConfiguration
        {
            PreAuthorizationMiddleware = async (ctx, next) =>
            {
                var apiKey =
                    ctx.RequestServices.GetRequiredService<ApiKeyGatewayMiddleware>();

                var jwt =
                    ctx.RequestServices.GetRequiredService<JwtGatewayMiddleware>();

                await apiKey.InvokeAsync(ctx);

                if (!ctx.Request.Headers.TryGetValue("X-Auth-Type", out var t)
                    || t != "apikey")
                {
                    await jwt.InvokeAsync(ctx);
                }

                await next();
            },

            PreErrorResponderMiddleware = async (ctx, next) =>
            {
                var errorMiddleware =
                    ctx.RequestServices.GetRequiredService<GatewayErrorEnvelopeMiddleware>();

                await errorMiddleware.InvokeAsync(ctx, next);
            }
        };

        app.UseOcelot(ocelotPipeline).Wait();

        app.Run();
    }
}
