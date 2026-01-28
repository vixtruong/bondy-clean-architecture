using Bondy.SharedKernel.Application.Abstractions.Security;
using Bondy.SharedKernel.Domain.Abstractions;
using Bondy.SharedKernel.Infrastructure.Common.Clock;
using Bondy.SharedKernel.Infrastructure.Configuration;
using Bondy.SharedKernel.Infrastructure.Security;
using Identity.Application.Abstractions.Integrations;
using Identity.Application.Abstractions.OAuth2;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Abstractions.Repositories;
using Identity.Application.Abstractions.Security;
using Identity.Infrastructure.Common.Security;
using Identity.Infrastructure.Integrations.Mail;
using Identity.Infrastructure.Integrations.OAuth2;
using Identity.Infrastructure.Jobs.Migration;
using Identity.Infrastructure.Jobs.Seed;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Persistence.Seed;
using Identity.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Storage.Abstractions;
using Shared.Storage.Local;

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

            opt.ConfigureWarnings(w =>
                w.Ignore(RelationalEventId.MultipleCollectionIncludeWarning));
        });

        services.AddScoped<IIdentityDbContext>(sp => sp.GetRequiredService<IdentityDbContext>());

        // Current User
        services.AddScoped<ICurrentUser, HttpContextCurrentUser>();

        // repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IPreRegistrationRepository, PreRegistrationRepository>();
        services.AddScoped<IOtpCodeRepository, OtpCodeRepository>();
        services.AddScoped<IApiKeyRepository, ApiKeyRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();

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

        services.AddScoped<IGoogleVerifier, GoogleVerifier>();
        services.AddScoped<IDiscordVerifier, DiscordVerifier>();
        services.AddScoped<ITempPasswordGenerator, TempPasswordGenerator>();

        services.AddHostedService<IdentityMigrationService>();

        services.AddScoped<RoleSeeder>();
        services.AddHostedService<RoleSeedHostedService>();

        services.AddScoped<IFileStorage, LocalFileStorage>();

        return services;
    }
}