using Mail.Application.Services.Mail;
using Microsoft.Extensions.DependencyInjection;

namespace Mail.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddMailApplication(this IServiceCollection services)
    {
        // Application services (use-cases)
        services.AddScoped<IMailService, MailService>();

        // Nếu bạn dùng MediatR/FluentValidation sau này thì add ở đây luôn
        // services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        return services;
    }
}
