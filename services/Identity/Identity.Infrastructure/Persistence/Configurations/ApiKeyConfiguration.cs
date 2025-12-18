using Identity.Domain.Entities;
using Identity.Domain.ValueObjects;
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

        b.Property(x => x.KeyHash)
            .HasConversion(
                v => v.Value,
                v => HashedValue.Create(v))
            .HasColumnName("key_hash")
            .IsRequired();

        b.HasIndex(x => x.KeyHash).IsUnique();


        b.Property(x => x.Prefix).HasColumnName("prefix");
        b.HasIndex(x => x.Prefix).IsUnique();

        b.Property(x => x.ExpiresAt).HasColumnName("expires_at");
        b.Property(x => x.Active).HasColumnName("active").IsRequired();

        b.HasIndex(x => new { x.Active, x.ExpiresAt }).HasDatabaseName("idx_api_keys_active");
        b.HasIndex(x => x.KeyHash).HasDatabaseName("idx_api_keys_prefix");
    }
}