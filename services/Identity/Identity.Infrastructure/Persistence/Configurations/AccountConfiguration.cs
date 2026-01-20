using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Infrastructure.Persistence.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Persistence.Configurations;

public sealed class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> b)
    {
        b.ToTable("accounts");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("id");

        b.Property<long>("UserId")
            .HasColumnName("user_id")
            .IsRequired();

        b.HasIndex("UserId")
            .HasDatabaseName("idx_accounts_user_id");

        b.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasConversion(Converter.UtcConverter)
            .IsRequired();

        b.Ignore(x => x.UpdatedAt);

        b.Property(x => x.Provider)
            .HasColumnName("provider")
            .HasConversion(
                v => v == AuthProvider.Local ? "LOCAL" : v.ToString().ToUpperInvariant(),
                v => Enum.Parse<AuthProvider>(v, true))
            .HasMaxLength(20)
            .IsRequired();

        b.OwnsOne(x => x.PasswordHash, h =>
        {
            h.Property(p => p.Value)
                .HasColumnName("password_hash");
        });
    }
}
