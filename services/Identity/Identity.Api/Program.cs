using Bondy.ServiceDefaults;
using Identity.Application;
using Identity.Infrastructure;

namespace Identity.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var serviceName = "Identity";

        var builder = WebApplication.CreateBuilder(args);

        builder.AddBondyServiceDefaults();

        // Identity
        builder.Services.AddIdentityApplication();

        builder.Services.AddHttpContextAccessor(); // Current User
        builder.Services.AddIdentityInfrastructure(builder.Configuration);

        var app = builder.Build();

        app.UseBondyServiceDefaults(builder, serviceName);

        app.MapControllers();

        app.Run();
    }
}