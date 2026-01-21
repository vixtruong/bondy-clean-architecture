// Identity.Infrastructure/Persistence/Configurations/UserConfiguration.cs (updated)
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Identity.Domain.Entities;
using Identity.Domain.ValueObjects;

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

        b.Property(x => x.Email)
            .HasConversion(v => v.Value, v => Email.FromPersisted(v))
            .HasColumnName("email")
            .IsRequired();

        b.HasIndex(x => x.Email).IsUnique().HasDatabaseName("ux_users_email");

        b.OwnsOne(x => x.Name, n =>
        {
            n.Property(p => p.FirstName).HasColumnName("first_name").IsRequired();
            n.Property(p => p.MiddleName).HasColumnName("middle_name");
            n.Property(p => p.LastName).HasColumnName("last_name");
        });

        b.Property(x => x.AvatarUrl).HasColumnName("avatar_url");
        b.Property(x => x.Dob).HasColumnName("dob");
        b.Property(x => x.Gender).HasColumnName("gender");
        b.Property(x => x.Active).HasColumnName("active").IsRequired();

        // user_roles many-to-many
        b.HasMany(u => u.Roles)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "user_roles",
                r => r.HasOne<Role>()
                    .WithMany()
                    .HasForeignKey("role_id")
                    .HasConstraintName("fk_user_roles_role"),
                l => l.HasOne<User>()
                    .WithMany()
                    .HasForeignKey("user_id")
                    .HasConstraintName("fk_user_roles_user"),
                je =>
                {
                    je.ToTable("user_roles");
                    je.HasKey("user_id", "role_id");
                });



        // granted scopes
        b.OwnsMany(x => x.GrantedScopes, sb =>
        {
            sb.ToTable("user_granted_scopes");
            sb.WithOwner().HasForeignKey("user_id");
            sb.Property(s => s.Value).HasColumnName("scope").HasMaxLength(500).IsRequired();
            sb.HasKey("user_id", nameof(Scope.Value));
            sb.HasIndex("user_id");
        });

        // denied scopes
        b.OwnsMany(x => x.DeniedScopes, sb =>
        {
            sb.ToTable("user_denied_scopes");
            sb.WithOwner().HasForeignKey("user_id");
            sb.Property(s => s.Value).HasColumnName("scope").HasMaxLength(500).IsRequired();
            sb.HasKey("user_id", nameof(Scope.Value));
            sb.HasIndex("user_id");
        });

        // Accounts and RefreshTokens mapping preserved as you had
        b.HasMany(x => x.Accounts)
            .WithOne("User")
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Cascade);

        b.HasMany(x => x.RefreshTokens)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
