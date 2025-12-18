using Identity.Domain.Entities;
using Identity.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Persistence.Configurations;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> b)
    {
        b.ToTable("refresh_tokens");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("id");

        b.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
        b.HasIndex(x => x.UserId).HasDatabaseName("idx_refresh_tokens_user_id");

        b.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        b.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        // TokenHash (1 cột) -> conversion
        b.Property(x => x.TokenHash)
            .HasConversion(v => v.Value, v => HashedValue.Create(v))
            .HasColumnName("token_hash")
            .IsRequired();

        // nếu bạn hay lookup theo token_hash thì nên có index
        b.HasIndex(x => x.TokenHash).HasDatabaseName("idx_refresh_tokens_token_hash");

        b.Property(x => x.Revoked).HasColumnName("revoked").IsRequired();
        b.Property(x => x.RevokedAt).HasColumnName("revoked_at");
        b.Property(x => x.ExpiresAt).HasColumnName("expires_at").IsRequired();
    }
}