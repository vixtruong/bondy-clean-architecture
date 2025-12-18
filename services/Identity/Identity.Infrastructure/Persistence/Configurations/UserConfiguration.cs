using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.ValueObjects;
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

        b.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        b.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        // Email (1 cột) -> conversion để index/query dễ
        b.Property(x => x.Email)
            .HasConversion(v => v.Value, v => Email.Create(v)) // đổi Create nếu factory khác
            .HasColumnName("email")
            .IsRequired();

        b.HasIndex(x => x.Email)
            .IsUnique()
            .HasDatabaseName("ux_users_email");

        // PersonName (nhiều cột) -> owns one OK
        b.OwnsOne(x => x.Name, n =>
        {
            n.Property(p => p.FirstName).HasColumnName("first_name").IsRequired();
            n.Property(p => p.MiddleName).HasColumnName("middle_name");
            n.Property(p => p.LastName).HasColumnName("last_name").IsRequired();
        });

        b.Property(x => x.AvatarUrl).HasColumnName("avatar_url");
        b.Property(x => x.Dob).HasColumnName("dob");
        b.Property(x => x.Gender).HasColumnName("gender");

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

        b.HasMany(x => x.Accounts)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasMany(x => x.RefreshTokens)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
