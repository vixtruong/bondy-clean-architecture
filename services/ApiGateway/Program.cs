using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;
using Serilog;
using System.Text;

namespace ApiGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Host.UseSerilog((ctx, services, loggerConfig) =>
            {
                loggerConfig
                    .ReadFrom.Configuration(ctx.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("Service", builder.Environment.ApplicationName);
            });

            builder.Configuration
                .AddJsonFile("ocelot.Development.json", optional: false, reloadOnChange: true)
                .AddJsonFile("swagger.Development.json", optional: false, reloadOnChange: true);

            //// JWT auth 
            //var jwt = builder.Configuration.GetSection("Jwt");
            //var issuer = jwt["Issuer"];
            //var audience = jwt["Audience"];
            //var secret = jwt["Secret"]!;

            //builder.Services
            //    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //    .AddJwtBearer("Bearer", options =>
            //    {
            //        options.RequireHttpsMetadata = true;
            //        options.TokenValidationParameters = new TokenValidationParameters
            //        {
            //            ValidateIssuer = true,
            //            ValidateAudience = true,
            //            ValidateLifetime = true,
            //            ValidateIssuerSigningKey = true,
            //            ValidIssuer = issuer,
            //            ValidAudience = audience,
            //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
            //        };
            //    });

            builder.Services.AddAuthorization();

            builder.Services
                .AddOcelot(builder.Configuration)
                .AddPolly();

            builder.Services.AddSwaggerForOcelot(builder.Configuration);

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                // Swagger UI
                app.UseSwaggerForOcelotUI(opt =>
                    {
                        opt.PathToSwaggerGenerator = "/swagger/docs";
                    })
                    .UseOcelot().Wait();
                
            }
            else
            {
                app.UseHttpsRedirection();
            }

            //app.UseAuthentication();
            app.UseAuthorization();

            

            app.Run();
        }
    }
}
