using Bondy.ServiceDefaults.Extensions;
using Bondy.ServiceDefaults.Middlewares;
using Mail.Application;
using Mail.Infrastructure;

namespace Mail.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.AddSerilogLogging();

            builder.Services
                .AddControllers()
                .AddServiceValidation();

            builder.Services.AddServiceSwagger();

            builder.Services.AddServiceHealthChecks(builder.Configuration);

            builder.Services.AddTransient<GlobalExceptionMiddleware>();

            builder.Services.AddMailApplication();
            builder.Services.AddMailInfrastructure(builder.Configuration, builder.Environment);

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
