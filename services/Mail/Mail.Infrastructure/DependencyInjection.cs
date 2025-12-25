using Bondy.SharedKernel.Abstractions;
using Mail.Application.Abstractions.Persistence;
using Mail.Application.Abstractions.Persistence.Migrations;
using Mail.Application.Abstractions.Repositories;
using Mail.Application.Abstractions.Templating;
using Mail.Infrastructure.Common.Clock;
using Mail.Infrastructure.Persistence;
using Mail.Infrastructure.Persistence.Migrations;
using Mail.Infrastructure.Repositories;
using Mail.Infrastructure.Templating;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using System.Data.Common;

namespace Mail.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddMailInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment env)
    {
        // DbContext
        services.AddDbContext<MailDbContext>(opt =>
        {
            var cs = configuration.GetConnectionString("MailDb");
            opt.UseNpgsql(cs);
        });

        services.AddScoped<IMailDbContext>(sp =>
            sp.GetRequiredService<MailDbContext>());

        // Native DbConnection (cho SQL migrator)
        services.AddScoped<DbConnection>(_ =>
            new NpgsqlConnection(configuration.GetConnectionString("MailDb")));

        // Native SQL migrator
        services.AddScoped<IDbMigrator, SqlFileMigrator>();
        services.AddHostedService<MigrationHostedService>();

        // Repositories
        services.AddScoped<IMailRepository, MailRepository>();

        // Common
        services.AddSingleton<IClock, SystemClock>();

        // SMTP options (fail-fast)
        services.AddOptions<SmtpOptions>()
            .Bind(configuration.GetSection(SmtpOptions.SectionName))
            .Validate(o =>
                !string.IsNullOrWhiteSpace(o.Host) &&
                !string.IsNullOrWhiteSpace(o.FromEmail),
                "SMTP options are invalid (Host/FromEmail are required).")
            .ValidateOnStart();

        services.AddScoped<IEmailSender, EmailSender>();

        // Templates
        services.AddSingleton<ITemplateProvider>(_ =>
        {
            var basePath = configuration["Mail:Templates:BasePath"];
            if (string.IsNullOrWhiteSpace(basePath))
                basePath = Path.Combine(env.ContentRootPath, "Templates");

            return new FileTemplateProvider(basePath);
        });

        services.AddSingleton<ITemplateRenderer, ScribanTemplateRenderer>();

        return services;
    }
}
