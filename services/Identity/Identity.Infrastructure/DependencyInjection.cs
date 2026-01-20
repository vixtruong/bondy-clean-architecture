using Bondy.SharedKernel.Abstractions;
using Bondy.SharedKernel.Configuration;
using Identity.Application.Abstractions.Integrations;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Abstractions.Repositories;
using Identity.Application.Abstractions.Security;
using Identity.Infrastructure.Common.Clock;
using Identity.Infrastructure.Common.Security;
using Identity.Infrastructure.Integrations.Mail;
using Identity.Infrastructure.Jobs.Migration;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<AppConfigOptions>(configuration.GetSection(AppConfigOptions.SectionName));

        services.AddDbContext<IdentityDbContext>(opt =>
        {
            var cs = configuration.GetConnectionString("IdentityDb");
            opt.UseNpgsql(cs, npsql =>
                {
                    npsql.MigrationsAssembly(typeof(DependencyInjection).Assembly.GetName().Name);
                })
                .UseSnakeCaseNamingConvention();
        });

        services.AddScoped<IIdentityDbContext>(sp => sp.GetRequiredService<IdentityDbContext>());

        // repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IPreRegistrationRepository, PreRegistrationRepository>();
        services.AddScoped<IOtpCodeRepository, OtpCodeRepository>();
        services.AddScoped<IApiKeyRepository, ApiKeyRepository>();

        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        services.AddSingleton<ITokenGenerator, TokenGenerator>();

        services.AddSingleton<IHasher, BcryptHasher>();
        services.AddSingleton<IClock, SystemClock>();

        // Mail Client
        services.AddOptions<MailClientOptions>()
            .Bind(configuration.GetSection(MailClientOptions.SectionName))
            .Validate(o => !string.IsNullOrWhiteSpace(o.BaseUrl), "Mail BaseUrl is required")
            .ValidateOnStart();

        services.AddHttpClient<IMailClient, MailClient>((sp, http) =>
        {
            var opt = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<MailClientOptions>>().Value;
            http.BaseAddress = new Uri(opt.BaseUrl);
            http.Timeout = TimeSpan.FromSeconds(10);
        });

        services.AddScoped<IOtpGenerator, OtpGenerator>();
        services.AddScoped<IApiKeyHasher, ApiKeyHasher>();
        services.AddScoped<IApiKeyGenerator, ApiKeyGenerator>();

        services.AddHostedService<IdentityMigrationService>();

        return services;
    }
}