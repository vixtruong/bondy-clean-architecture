using ApiGateway.Authentication;
using ApiGateway.Clients.Identity;
using ApiGateway.Middlewares.Auth;
using ApiGateway.Middlewares.Error;
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

        builder.Services.AddGatewayAuthentication(builder.Configuration);

        builder.Services.AddOcelot(builder.Configuration).AddPolly();
        builder.Services.AddSwaggerForOcelot(builder.Configuration);

        // register middlewares as transient or scoped
        //builder.Services.AddTransient<ApiKeyGatewayMiddleware>();
        //builder.Services.AddTransient<JwtGatewayMiddleware>();
        //builder.Services.AddTransient<GatewayErrorEnvelopeMiddleware>();

        builder.Services.AddHttpClient<IIdentityClient, IdentityClient>(client =>
        {
            client.BaseAddress = new Uri(builder.Configuration["Services:Identity"]!);
            client.Timeout = TimeSpan.FromSeconds(5);
        });

        builder.Services.AddHttpContextAccessor();

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

        // Register our middlewares BEFORE UseOcelot so headers exist for Ocelot to forward
        //app.UseMiddleware<ApiKeyGatewayMiddleware>();
        //app.UseMiddleware<Middlewares.Auth.JwtGatewayMiddleware>();

        app.UseMiddleware<IdentityForwardingMiddleware>();

        // Error envelope should wrap Ocelot call, so register it before UseOcelot.
        app.UseMiddleware<GatewayErrorEnvelopeMiddleware>();

        // No Ocelot pipeline modifications here — Use default Ocelot pipeline
        app.UseOcelot().Wait();

        app.Run();
    }
}
