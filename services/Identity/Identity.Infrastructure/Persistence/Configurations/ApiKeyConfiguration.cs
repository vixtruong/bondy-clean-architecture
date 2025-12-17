using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Persistence.Configurations;

public sealed class ApiKeyConfiguration : IEntityTypeConfiguration<ApiKey>
{
    public void Configure(EntityTypeBuilder<ApiKey> b)
    {
        b.ToTable("api_keys");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("id");

        b.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        b.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        b.Property(x => x.Name).HasColumnName("name").HasMaxLength(100).IsRequired();

        b.OwnsOne(x => x.KeyHash, h =>
        {
            h.Property(p => p.Value).HasColumnName("key_hash").HasMaxLength(64).IsRequired();
        });
        b.HasIndex("key_hash").IsUnique();

        b.OwnsOne(x => x.Prefix, p =>
        {
            p.Property(v => v.Value).HasColumnName("prefix").HasMaxLength(12).IsRequired();
        });
        b.HasIndex("prefix").IsUnique();

        b.Property(x => x.ExpiresAt).HasColumnName("expires_at");
        b.Property(x => x.Active).HasColumnName("active").IsRequired();

        b.HasIndex(x => new { x.Active, x.ExpiresAt }).HasDatabaseName("idx_api_keys_active");
        b.HasIndex("prefix").HasDatabaseName("idx_api_keys_prefix");
    }
}