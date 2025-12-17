using Bondy.ServiceDefaults.Extensions;
using Bondy.ServiceDefaults.Middlewares;
using Identity.Application;
using Identity.Infrastructure;

namespace Identity.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.AddSerilogLogging();

            builder.Services.AddControllers();

            builder.Services.AddServiceSwagger();

            //builder.Services.AddJwtAuth(builder.Configuration);

            builder.Services.AddServiceHealthChecks(builder.Configuration);

            builder.Services.AddTransient<GlobalExceptionMiddleware>();

            builder.Services.AddIdentityApplication();
            builder.Services.AddIdentityInfrastructure(builder.Configuration);

            var app = builder.Build();

            app.UseMiddleware<GlobalExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseServiceSwagger();
            }
            else
            {
                // Configure the HTTP request pipeline.

                app.UseHttpsRedirection();
            }

            //app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapServiceHealthChecks();

            app.Run();
        }
    }
}
