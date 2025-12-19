using Mail.Application.Abstractions.Persistence;
using Mail.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mail.Infrastructure.Persistence
{
    public class MailDbContext : DbContext, IMailDbContext
    {
        public MailDbContext(DbContextOptions<MailDbContext> options) : base(options) { }

        public DbSet<EmailLog> EmailLogs => Set<EmailLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MailDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
