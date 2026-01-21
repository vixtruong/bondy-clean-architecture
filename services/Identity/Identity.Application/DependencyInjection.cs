using Identity.Application.Services.ApiKey;
using Identity.Application.Services.Auth;
using Identity.Application.Services.User;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityApplication(this IServiceCollection services)
    {
        // Application services (use-cases)
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IApiKeyService, ApiKeyService>();
        services.AddScoped<IUserService, UserService>();

        // Nếu bạn dùng MediatR/FluentValidation sau này thì add ở đây luôn
        // services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        return services;
    }
}