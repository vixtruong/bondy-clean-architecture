using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Persistence.Utils;
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

        b.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasConversion(Converter.UtcConverter)
            .IsRequired();

        b.Property(x => x.UpdatedAt)
            .HasColumnName("last_used_at")
            .HasConversion(Converter.UtcConverter);

        b.Property(x => x.KeyId)
            .HasColumnName("key_id")
            .HasMaxLength(50)
            .IsRequired();

        b.HasIndex(x => x.KeyId).IsUnique();

        b.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        b.Property(x => x.KeyPrefix)
            .HasColumnName("key_prefix")
            .HasMaxLength(100)
            .IsRequired();

        b.Property(x => x.KeyHash)
            .HasConversion(
                v => v.Value,
                v => HashedValue.FromPersisted(v))
            .HasColumnName("key_hash")
            .IsRequired();

        b.HasIndex(x => x.KeyHash).IsUnique();

        b.Property(x => x.Owner)
            .HasColumnName("owner")
            .HasMaxLength(100)
            .IsRequired();

        b.Property(x => x.OwnerEmail)
            .HasConversion(v => v.Value, v => Email.FromPersisted(v))
            .HasColumnName("owner_email")
            .IsRequired();

        b.Property(x => x.AllowedPaths)
            .HasColumnName("allowed_paths")
            .HasMaxLength(500);

        b.Property(x => x.RateLimitPlanId)
            .HasColumnName("rate_limit_plan_id");

        b.Property(x => x.ExpiresAt)
            .HasColumnName("expires_at");

        b.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        b.HasIndex(x => new { x.IsActive, x.ExpiresAt })
            .HasDatabaseName("idx_api_keys_active_exp");

        b.OwnsMany(x => x.Scopes, sb =>
        {
            sb.ToTable("api_key_scopes");

            sb.WithOwner().HasForeignKey("api_key_id");

            sb.Property(s => s.Value)
                .HasColumnName("scope")
                .HasMaxLength(500)
                .IsRequired();

            sb.HasKey("api_key_id", nameof(Scope.Value));
        });

        b.Property(x => x.RevokeAt)
            .HasColumnName("revoke_at");


        b.Property(x => x.RotateAt)
            .HasColumnName("rotate_at");

        b.Property(x => x.RevokeReason)
            .HasColumnName("revoke_reason")
            .HasMaxLength(50)
            .HasConversion(
                v => v != null ? v.ToString() : null,
                v => v != null ? Enum.Parse<ApiKeyRevokeReason>(v) : null
            );

        b.HasIndex(x => x.RevokeReason)
            .HasDatabaseName("idx_api_keys_revoke_reason");
    }
}
