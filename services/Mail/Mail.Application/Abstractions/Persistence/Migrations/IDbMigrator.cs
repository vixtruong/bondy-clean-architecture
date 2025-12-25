namespace Mail.Application.Abstractions.Persistence.Migrations
{
    public interface IDbMigrator
    {
        Task MigrateAsync(CancellationToken ct);
    }
}
