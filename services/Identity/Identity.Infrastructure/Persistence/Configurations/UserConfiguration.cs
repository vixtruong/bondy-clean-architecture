using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Persistence.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.ToTable("users");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("id");

        b.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasConversion(Converter.UtcConverter)
            .IsRequired();

        b.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .HasConversion(Converter.UtcConverter);

        // Email
        b.Property(x => x.Email)
            .HasConversion(v => v.Value, v => Email.FromPersisted(v))
            .HasColumnName("email")
            .IsRequired();

        b.HasIndex(x => x.Email)
            .IsUnique()
            .HasDatabaseName("ux_users_email");

        // PersonName (ValueObject)
        b.OwnsOne(x => x.Name, n =>
        {
            n.Property(p => p.FirstName).HasColumnName("first_name").IsRequired();
            n.Property(p => p.MiddleName).HasColumnName("middle_name");
            n.Property(p => p.LastName).HasColumnName("last_name").IsRequired();
        });

        b.Property(x => x.AvatarUrl).HasColumnName("avatar_url");
        b.Property(x => x.Dob).HasColumnName("dob").HasConversion(Converter.UtcConverter);
        b.Property(x => x.Gender).HasColumnName("gender");

        // Role chỉ để grouping
        b.Property(x => x.Role)
            .HasColumnName("role")
            .HasConversion(
                v => v == UserRole.Admin ? "ADMIN" : "USER",
                v => v == "ADMIN" ? UserRole.Admin : UserRole.User)
            .HasMaxLength(10)
            .IsRequired();

        b.ToTable(t => t.HasCheckConstraint("ck_users_role", "role IN ('USER','ADMIN')"));

        b.Property(x => x.Active).HasColumnName("active").IsRequired();
        b.Property(x => x.FriendCount).HasColumnName("friend_count").IsRequired();

        // ===== USER SCOPES (JWT sẽ phát từ đây) =====
        // ===== USER SCOPES (JWT sẽ phát từ đây) =====
        b.OwnsMany(x => x.Scopes, sb =>
        {
            sb.ToTable("user_scopes");

            sb.WithOwner().HasForeignKey("user_id");

            sb.Property(s => s.Value)
                .HasColumnName("scope")
                .HasMaxLength(500)
                .IsRequired();

            sb.HasKey("user_id", nameof(Scope.Value));
        });

        // Accounts
        b.HasMany(x => x.Accounts)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // RefreshTokens
        b.HasMany(x => x.RefreshTokens)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
