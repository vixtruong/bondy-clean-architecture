using Bondy.ServiceDefaults;
using Mail.Application;
using Mail.Infrastructure;

namespace Mail.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var serviceName = "Mail";

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.AddBondyServiceDefaults();

            builder.Services.AddMailApplication();
            builder.Services.AddMailInfrastructure(builder.Configuration, builder.Environment);

            var app = builder.Build();

            app.UseBondyServiceDefaults(builder, serviceName);
            
            app.MapControllers();
            app.Run();
        }
    }
}
